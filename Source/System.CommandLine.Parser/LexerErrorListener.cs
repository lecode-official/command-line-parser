
#region Using Directives

using Antlr4.Runtime;
using System.Collections.Generic;
using System.CommandLine.Parser.Antlr;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a listener, which listens to errors during the lexing of the input and makes errors available in a simple way.
    /// </summary>
    internal class LexerErrorListener : IAntlrErrorListener<int>
    {
        #region IAntlrErrorListener Implementation

        /// <summary>
        /// Is invoked when a lexical error is detected in the lexer.
        /// </summary>
        /// <param name="recognizer">The lexer that found the lexical error.</param>
        /// <param name="offendingSymbol">The ID of the symbol that caused the error.</param>
        /// <param name="line">The line number where the error occurred.</param>
        /// <param name="charPositionInLine">The index of the character where the error starts within the current line.</param>
        /// <param name="msg">The error message that was produced.</param>
        /// <param name="e">The original exception that was thrown when the error occurred.</param>
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => this.errors.Add(new ParserError(charPositionInLine, (recognizer as CommandLineLexer).Text, ParserErrorKind.Lexical));

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains all the lexical errors that have been found.
        /// </summary>
        private List<ParserError> errors = new List<ParserError>();

        /// <summary>
        /// Gets all the lexical errors that have been found.
        /// </summary>
        public IEnumerable<ParserError> Errors
        {
            get
            {
                return this.errors;
            }
        }

        #endregion
    }
}