﻿
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.CommandLine.Parser.Antlr;
using System.IO;
using System.Linq;

#endregion

namespace System.CommandLine.Parser.UnitTests
{
    /// <summary>
    /// Represents a unit testing class, which contains all of the unit tests for the command line parser.
    /// </summary>
    [TestClass]
    public class AntlrParserUnitTests
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
        /// Parses the specified token stream.
        /// </summary>
        /// <param name="lexer">The lexer, which lexes the token consumed by the parser.</param>
        /// <returns>Returns the parse tree generated by parsing the specified token stream.</returns>
        private IParseTree ParseTokens(ITokenSource lexer)
        {
            // Parses the token stream
            CommandLineParser parser = new CommandLineParser(new CommonTokenStream(lexer));
            parser.BuildParseTree = true;
            IParseTree parseTree = parser.commandLine();

            // Returns the generated parse tree
            return parseTree;
        }

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

        #region General Test Methods

        /// <summary>
        /// Test if the ANTLR4 lexer and parser can handle empty command line parameters.
        /// </summary>
        [TestMethod]
        public void EmptyCommandLineParamentersTest()
        {
            // Lexes the input and checks whether there are no tokens
            CommandLineLexer lexer = this.LexInput(string.Empty);
            Assert.AreEqual(lexer.NextToken().Type, TokenConstants.Eof);

            // Parses the tokens and checks whether the resulting tree is empty
            IParseTree tree = this.ParseTokens(lexer);
            Assert.AreEqual(tree.ChildCount, 0);
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