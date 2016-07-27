
namespace System.CommandLine.Parser.Parameters
{
    /// <summary>
    /// Represents the abstract base class for the typed command line parameters.
    /// </summary>
    public abstract class Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the kind of the parameter.
        /// </summary>
        public abstract ParameterKind Kind { get; }

        #endregion
    }
}