
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents the abstract base class for the typed command line parameters.
    /// </summary>
    public abstract class Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the name of the command line parameter.
        /// </summary>
        public virtual string Name { get; internal set; }

        /// <summary>
        /// Gets a value that determines whether this parameter is a default parameter.
        /// </summary>
        public virtual bool IsDefaultParameter { get; } = false;

        #endregion
    }
}