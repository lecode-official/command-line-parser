﻿
#region Using Directives

using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.CommandLine.Parser.Antlr;
using System.IO;
using System.Linq;

#endregion

namespace System.CommandLine.Parser.UnitTests
{
    /// <summary>
    /// Represents a unit testing class, which contains all of the unit tests for the command line lexer.
    /// </summary>
    [TestClass]
    public class AntlrLexerUnitTests
    {
        #region Private Methods

        /// <summary>
        /// Lexes the specified input.
        /// </summary>
        /// <param name="input">The input that is to be lexed.</param>
        /// <returns>Returns the lexed tokens from the specified input.</returns>
        private CommandLineLexer LexInput(string input)
        {
            // Creates a new stream input for the input string
            AntlrInputStream stream = new AntlrInputStream(new StringReader(input));

            // Lexes the input and returns the lexer
            CommandLineLexer lexer = new CommandLineLexer(stream);
            return lexer;
        }
        
        /// <summary>
        /// Retrieves the name of the token type (which is equivalent to the rule by which it was recognized) for the specified token.
        /// </summary>
        /// <param name="lexer">The lexer that was used to scan the specified token.</param>
        /// <param name="token">The token for which the name of the token type is to be retrieved.</param>
        /// <returns>Returns the token type for the specified token or <c>null</c> if the token type could not be determined.</returns>
        private string GetTokenTypeName(CommandLineLexer lexer, IToken token)
        {
            // Since the token type map may contain multiple versions of the same token type (because there may be explicit and implicit versions of the token
            // in the grammar), all of them are retrieved
            IEnumerable<string> tokenTypeNames = lexer.TokenTypeMap.Where(keyValuePair => keyValuePair.Value == token.Type).Select(keyValuePair => keyValuePair.Key);

            // Gets the first token type that is not implicit, but an explicit version of the token type, which is when it does not start and end with "'",
            // if such an explicit token type was found, then it is returned
            string explicitTokenTypeName = tokenTypeNames.FirstOrDefault(tokenTypeName => !tokenTypeName.StartsWith("'") && !tokenTypeName.EndsWith("'"));
            if (!string.IsNullOrWhiteSpace(explicitTokenTypeName))
                return explicitTokenTypeName;

            // If no explicit token type was found, then the first one is returned
            return tokenTypeNames.FirstOrDefault();
        }

        #endregion

        #region Data Type Test Methods

        /// <summary>
        /// Tests how the ANTLR4 lexer handles the lexing of boolean values.
        /// </summary>
        [TestMethod]
        public void BooleanDataTypeTest()
        {
            // Lexes the boolean value true and checks if the correct token was recognized
            CommandLineLexer lexer = this.LexInput("true");
            IToken token = lexer.NextToken();
            Assert.AreEqual(token.Text, "true");
            string tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "True");

            // Lexes the boolean value false and checks if the correct token was recognized
            lexer = this.LexInput("false");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "false");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "False");
        }

        /// <summary>
        /// Tests how the ANTLR4 lexer handles the lexing of numbers.
        /// </summary>
        [TestMethod]
        public void NumberDataTypeTest()
        {
            // Lexes a positive integer and checks if the correct token was recognized
            CommandLineLexer lexer = this.LexInput("123");
            IToken token = lexer.NextToken();
            Assert.AreEqual(token.Text, "123");
            string tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a negative integer and checks if the correct token was recognized
            lexer = this.LexInput("-123");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "-123");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a positive floating point number and checks if the correct token was recognized
            lexer = this.LexInput("123.456");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "123.456");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a negative floating point number and checks if the correct token was recognized
            lexer = this.LexInput("-123.456");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "-123.456");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a positive floating point number with no digits before the decimal point and checks if the correct token was recognized
            lexer = this.LexInput(".123");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, ".123");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a negative floating point number with no digits before the decimal point and checks if the correct token was recognized
            lexer = this.LexInput("-.123");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "-.123");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a positive floating point number with no digits after the decimal point and checks if the correct token was recognized
            lexer = this.LexInput("123.");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "123.");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");

            // Lexes a negative floating point number with no digits after the decimal point and checks if the correct token was recognized
            lexer = this.LexInput("-123.");
            token = lexer.NextToken();
            Assert.AreEqual(token.Text, "-123.");
            tokenType = this.GetTokenTypeName(lexer, token);
            Assert.AreEqual(tokenType, "Number");
        }

        #endregion
    }
}