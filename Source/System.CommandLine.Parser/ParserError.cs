
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an error, which occurred while lexing or parsing the input.
    /// </summary>
    public class ParserError
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ParserError"/> instance.
        /// </summary>
        /// <param name="startIndex">The index at which the offending symbol starts.</param>
        /// <param name="offendingSymbol">The symbol that caused the error.</param>
        /// <param name="kind">The kind of error that happened.</param>
        public ParserError(int startIndex, string offendingSymbol, ParserErrorKind kind)
        {
            this.StartIndex = startIndex;
            this.OffendingSymbol = offendingSymbol;
            this.Kind = kind;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index at which the offending symbol starts.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the symbol that caused the error.
        /// </summary>
        public string OffendingSymbol { get; private set; }

        /// <summary>
        /// Gets the kind of error that happened.
        /// </summary>
        public ParserErrorKind Kind { get; private set; }

        #endregion
    }
}