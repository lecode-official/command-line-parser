
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an attribute, which can be used to specify the name of the command line parameter with which the property or the constructor parameter marked by this attribute is to be matched.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ParameterNameAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ParameterNameAttribute"/> instance.
        /// </summary>
        /// <param name="parameterName">The name of the command line parameter with which the property marked by this attribute is to be matched.</param>
        /// <param name="parameterAlias">The alias for the command line parameter with which the property marked by this attribute can be alternatively matched.</param>
        /// <exception cref="ArgumentNullException">If the parameter name if <c>null</c> or white space, then a <see cref="ArgumentNullException"/> exception is thrown.</exception>
        /// <exception cref="ArgumentException">If the parameter alias is not either <c>null</c> or a valid paramter name, then an <see cref="ArgumentException"/> exception is thrown.</exception>
        public ParameterNameAttribute(string parameterName, string parameterAlias)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException(nameof(parameterName));
            if (parameterAlias != null && parameterAlias.Trim() == string.Empty)
                throw new ArgumentException("The alias must either be null or a valid alias.", nameof(parameterAlias));

            // Stores the parameter name and the parameter alias for later use
            this.ParameterName = parameterName;
            this.ParameterAlias = parameterAlias;
        }

        /// <summary>
        /// Initializes a new <see cref="ParameterNameAttribute"/> instance.
        /// </summary>
        /// <param name="parameterName">The name of the command line parameter with which the property marked by this attribute is to be matched.</param>
        /// <exception cref="ArgumentNullException">If the parameter name if <c>null</c> or white space, then a <see cref="ArgumentNullException"/> exception is thrown.</exception>
        /// <exception cref="ArgumentException">If the parameter alias is not either <c>null</c> or a valid paramter name, then an <see cref="ArgumentException"/> exception is thrown.</exception>
        public ParameterNameAttribute(string parameterName)
            : this(parameterName, null)
        { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the command line parameter with which the property marked by this attribute is to be matched.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets or sets the alias for the command line parameter with which the property marked by this attribute can be alternatively matched.
        /// </summary>
        public string ParameterAlias { get; private set; }

        #endregion
    }
}