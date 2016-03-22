
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a string command line parameter.
    /// </summary>
    public class StringCommandLineParameter : CommandLineParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the string value of the command line parameter.
        /// </summary>
        public string Value { get; internal set; }

        #endregion
    }
}