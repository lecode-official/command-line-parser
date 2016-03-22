
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a default parameter.
    /// </summary>
    public class DefaultParameter : Parameter
    {
        #region Parameter Implementation

        /// <summary>
        /// Gets the name of the parameter, always returns "Default".
        /// </summary>
        public override string Name
        {
            get
            {
                return "Default";
            }

            internal set
            {
                throw new InvalidOperationException("The name of a default parameter can not be changed.");
            }
        }

        /// <summary>
        /// Gets a value that determines whether this parameter is a default parameter. Always returns <c>true</c>.
        /// </summary>
        public override bool IsDefaultParameter
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value of the default parameter.
        /// </summary>
        public string Value { get; internal set; }

        #endregion
    }
}