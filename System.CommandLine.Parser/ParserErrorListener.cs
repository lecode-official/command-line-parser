
#region Using Directives

using Antlr4.Runtime;
using System.Collections.Generic;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a listener, which listens to errors during the parsing of the input and makes errors available in a simple way.
    /// </summary>
    public class ParserErrorListener : BaseErrorListener
    {
        #region  BaseErrorListener Implementation

        /// <summary>
        /// Is invoked when a syntactical error is detected in the parser.
        /// </summary>
        /// <param name="recognizer">The parser that found the syntactical error.</param>
        /// <param name="offendingSymbol">The ID of the symbol that caused the error.</param>
        /// <param name="line">The line number where the error occurred.</param>
        /// <param name="charPositionInLine">The index of the character where the error starts within the current line.</param>
        /// <param name="msg">The error message that was produced.</param>
        /// <param name="e">The original exception that was thrown when the error occurred.</param>
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => this.errors.Add(new SyntacticalError(charPositionInLine, offendingSymbol.Text));

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains all the syntactical errors that have been found.
        /// </summary>
        private List<SyntacticalError> errors = new List<SyntacticalError>();

        /// <summary>
        /// Gets all the syntactical errors that have been found.
        /// </summary>
        public IEnumerable<SyntacticalError> Errors
        {
            get
            {
                return this.errors;
            }
        }

        #endregion
    }
}