
#region Using Directives

using System.CommandLine.Parser.Parameters;

#endregion

namespace System.CommandLine.Parser.ValueConverters
{
    /// <summary>
    /// Represents the interface that all parameter converters have to implement. Parameter converters are used to convert the parsed parameters into the real values that are
    /// injected into the POCO objects.
    /// </summary>
    public interface IParameterConverter
    {
        #region Methods

        /// <summary>
        /// Dertermines whether the specified parameter can be converted into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameters that is to be converted into the specified type.</param>
        /// <returns>Returns <c>true</c> if the specified parameter can be converted into the specified type and <c>false</c> otherwise.</returns>
        bool CanConvert(Type propertyType, Parameter parameter);

        /// <summary>
        /// Converts the specified parameter into the specified type.
        /// </summary>
        /// <typeparam name="T">The type into which the parameter is to be converted.</typeparam>
        /// <param name="parameter">The parameter that is to be converted into the specified type.</param>
        /// <returns>Returns the converted parameter value.</returns>
        T Convert<T>(Parameter parameter);

        #endregion
    }
}