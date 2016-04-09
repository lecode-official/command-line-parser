
namespace System.CommandLine.Parser.Parameters
{
    /// <summary>
    /// Represents a boolean command line parameter.
    /// </summary>
    public class BooleanParameter : Parameter
    {
        #region Public Properties

        /// <summary>
        /// Gets the kind of the parameter.
        /// </summary>
        public override ParameterKind Kind
        {
            get
            {
                return ParameterKind.Boolean;
            }
        }

        /// <summary>
        /// Gets the boolean value of the command line parameter.
        /// </summary>
        public bool Value { get; internal set; }

        #endregion
    }
}