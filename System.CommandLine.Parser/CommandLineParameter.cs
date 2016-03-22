
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents the abstract base class for the typed command line parameters.
    /// </summary>
    public abstract class CommandLineParameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the name of the command line parameter.
        /// </summary>
        public string Name { get; internal set; }
        
        #endregion
    }
}