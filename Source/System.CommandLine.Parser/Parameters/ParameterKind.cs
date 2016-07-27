
namespace System.CommandLine.Parser.Parameters
{
    /// <summary>
    /// Represents an enumeration for the different kinds of parameters.
    /// </summary>
    public enum ParameterKind
    {
        /// <summary>
        /// The parameter is of type number.
        /// </summary>
        Number,

        /// <summary>
        /// The parameter is of type string.
        /// </summary>
        String,

        /// <summary>
        /// The parameter is of type boolean.
        /// </summary>
        Boolean,

        /// <summary>
        /// The parameter is of type array.
        /// </summary>
        Array,

        /// <summary>
        /// The parameter is a default parameter.
        /// </summary>
        Default
    }
}