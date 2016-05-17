
#region Using Directives

using System.CommandLine.Parser.Parameters;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert parameters into enumeration types.
    /// </summary>
    public class EnumerationParameterConverter : IParameterConverter
    {
        #region IParameterConverter Implementation

        /// <summary>
        /// Dertermines whether the specified parameter can be converted into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameters that is to be converted into the specified type.</param>
        /// <returns>Returns <c>true</c> if the specified parameter can be converted into the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvert(Type propertyType, Parameter parameter)
        {
            // Checks if the property is of type boolean, other types are not supported
            if (!propertyType.IsEnum)
                return false;

            // Checks if the parameter is a string and can be converted into the enumeration
            if (parameter.Kind == ParameterKind.String && propertyType.IsEnumDefined((parameter as StringParameter).Value))
                return true;
            return false;
        }

        /// <summary>
        /// Converts the specified parameter into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameter that is to be converted into the specified type.</param>
        /// <exception cref="InvalidOperationException">If the parameter could not be converted, an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the converted parameter value.</returns>
        public object Convert(Type propertyType, Parameter parameter)
        {
            // Gets the value of the parameter, if the value could not be retrieved an exception is thrown
            object value;
            if (parameter.Kind == ParameterKind.String && propertyType.IsEnumDefined((parameter as StringParameter).Value))
                value = Enum.Parse(propertyType, (parameter as StringParameter).Value);
            else
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is not a string, or the value is not a valid value for the enumeration.");

            // Checks the type of the property, converts the value accordingly, and returns it
            if (propertyType.IsEnum)
                return value;

            // Since the value could not be converted, an exception is thrown
            throw new InvalidOperationException("The parameter could not be converted, because the property is of an enumeration type.");
        }

        #endregion
    }
}