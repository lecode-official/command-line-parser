
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a boolean command line parameter.
    /// </summary>
    public class BooleanCommandLineParameter : CommandLineParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the boolean value of the command line parameter.
        /// </summary>
        public bool Value { get; internal set; }

        #endregion
    }
}