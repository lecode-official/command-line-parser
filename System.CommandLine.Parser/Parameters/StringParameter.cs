
namespace System.CommandLine.Parser.Parameters
{
    /// <summary>
    /// Represents a string command line parameter.
    /// </summary>
    public class StringParameter : Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the kind of the parameter.
        /// </summary>
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.String;
            }
        }

        /// <summary>
        /// Gets the string value of the command line parameter.
        /// </summary>
        public string Value { get; internal set; }

        #endregion
    }
}