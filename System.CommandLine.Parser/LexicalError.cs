
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an error, which occurred while lexing the input character stream.
    /// </summary>
    public class LexicalError
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="LexicalError"/> instance.
        /// </summary>
        /// <param name="startIndex">The index at which the offending symbol starts.</param>
        /// <param name="offendingSymbol">The symbol that caused the lexical error.</param>
        public LexicalError(int startIndex, string offendingSymbol)
        {
            this.StartIndex = startIndex;
            this.OffendingSymbol = offendingSymbol;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index at which the offending symbol starts.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the symbol that caused the lexical error.
        /// </summary>
        public string OffendingSymbol { get; private set; }

        #endregion
    }
}