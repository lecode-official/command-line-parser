
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a default parameter.
    /// </summary>
    public class DefaultParameter : Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the value of the default parameter.
        /// </summary>
        public string Value { get; internal set; }

        #endregion
    }
}