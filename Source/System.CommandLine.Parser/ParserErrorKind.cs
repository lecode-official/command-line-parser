
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an enumeration for the different kinds of errors that can occur in the parser.
    /// </summary>
    public enum ParserErrorKind
    {
        /// <summary>
        /// The error occurred during the lexical analysis.
        /// </summary>
        Lexical,

        /// <summary>
        /// The error occurred during the syntactical analysis.
        /// </summary>
        Syntactical
    }
}