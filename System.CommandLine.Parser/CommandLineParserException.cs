
#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an exception, which is thrown when there is any error during the lexing or parsing.
    /// </summary>
    public class CommandLineParserException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="CommandLineParserException"/> instance.
        /// </summary>
        /// <param name="parserErrors">The parser error that occurred.</param>
        public CommandLineParserException(IEnumerable<ParserError> parserErrors)
            : this($"{parserErrors.Where(error => error.Kind == ParserErrorKind.Lexical).Count()} lexical error(s) and {parserErrors.Where(error => error.Kind == ParserErrorKind.Syntactical).Count()} syntactical error(s) occurred during the parsing of the command line parameters.")
        {
            this.ParserErrors = parserErrors;
        }

        /// <summary>
        /// Initializes a new <see cref="CommandLineParserException"/> instance.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CommandLineParserException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new <see cref="CommandLineParserException"/> instance.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.</param>
        public CommandLineParserException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new <see cref="CommandLineParserException"/> instance.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected CommandLineParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets all the parser error that occurred.
        /// </summary>
        public IEnumerable<ParserError> ParserErrors { get; private set; } = new List<ParserError>();

        #endregion
    }
}