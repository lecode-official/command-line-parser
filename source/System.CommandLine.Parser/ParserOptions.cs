
namespace System.CommandLine
{
    /// <summary>
    /// Represents a property bag for the different options for the command line parser.
    /// </summary>
    public class ParserOptions
    {
        #region Public Static Properties

        /// <summary>
        /// Contains some sensible default options for the command line parser.
        /// </summary>
        private static ParserOptions defaultOptions = new ParserOptions
        {
            IgnoreCase = true
        };

        /// <summary>
        /// Gets some sensible default options for the command line parser.
        /// </summary>
        public static ParserOptions Default
        {
            get => ParserOptions.defaultOptions;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that determines whether the parser ignores case or not when parsing argument names.
        /// </summary>
        public bool IgnoreCase { get; set; }

        #endregion
    }
}