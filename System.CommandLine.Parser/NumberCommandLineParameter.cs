
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a numeric command line parameter.
    /// </summary>
    public class NumberCommandLineParameter : CommandLineParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the numeric value of the command line parameter.
        /// </summary>
        public double Value { get; internal set; }

        #endregion
    }
}