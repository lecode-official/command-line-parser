
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents an attribute, which can be used to specify that the property or constructor parameter is to be matched with one or multiple default command line parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DefaultParameterAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="DefaultParameterAttribute"/> instance.
        /// </summary>
        public DefaultParameterAttribute()
        {
            this.DefaultParameterIndex = null;
        }

        /// <summary>
        /// Initializes a new <see cref="DefaultParameterAttribute"/> instance.
        /// </summary>
        /// <param name="defaultParameterIndex">The index of the default parameter that is to be matched with the property or constructor parameter that was marked with this attribute.</param>
        public DefaultParameterAttribute(int defaultParameterIndex)
        {
            this.DefaultParameterIndex = defaultParameterIndex;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index of the default parameter that is to be matched with the property or constructor parameter that was marked with this attribute. If set to <c>null</c>, all default parameters are matched with it.
        /// </summary>
        public Nullable<int> DefaultParameterIndex { get; private set; }

        #endregion
    }
}