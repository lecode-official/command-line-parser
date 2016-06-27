
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
        /// <param name="startIndex">The index at which the offending token starts.</param>
        /// <param name="offendingToken">The token that caused the lexical error.</param>
        public LexicalError(int startIndex, string offendingToken)
        {
            this.StartIndex = startIndex;
            this.OffendingToken = offendingToken;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index at which the offending token starts.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the token that caused the lexical error.
        /// </summary>
        public string OffendingToken { get; private set; }

        #endregion
    }
}