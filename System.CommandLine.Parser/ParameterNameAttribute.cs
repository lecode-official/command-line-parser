
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an attribute, which can be used to specify the name of the command line parameter with which the property marked by this attribute is to be matched.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ParameterNameAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ParameterNameAttribute"/> instance.
        /// </summary>
        /// <param name="parameterName">The name of the command line parameter with which the property marked by this attribute is to be matched.</param>
        public ParameterNameAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the command line parameter with which the property marked by this attribute is to be matched.
        /// </summary>
        public string ParameterName { get; private set; }

        #endregion
    }
}