
namespace System.CommandLine.Parser.Parameters
{
    /// <summary>
    /// Represents a numeric command line parameter.
    /// </summary>
    public class NumberParameter : Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the numeric value of the command line parameter.
        /// </summary>
        public double Value { get; internal set; }

        #endregion
    }
}