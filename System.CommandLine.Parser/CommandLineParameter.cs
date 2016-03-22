
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents the abstract base class for the typed command line parameters.
    /// </summary>
    public abstract class CommandLineParameter<T>
    {
        #region Public Properties

        /// <summary>
        /// Gets the name of the command line parameter.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value of the command line parameter.
        /// </summary>
        public T Value { get; internal set; }

        #endregion
    }
}