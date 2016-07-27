
#region Using Directives

using System.CommandLine.Parser.Parameters;
using System.Globalization;
using System.Text;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert parameters into string types.
    /// </summary>
    public class StringParameterConverter : IParameterConverter
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
            // Checks if the property is either of type string or of type string builder, other types are not supported
            if (propertyType != typeof(string) && propertyType != typeof(StringBuilder))
                return false;

            // Checks if the parameter belongs to one of the supported types (string, boolean, and number)
            if (parameter.Kind == ParameterKind.String || parameter.Kind == ParameterKind.Boolean || parameter.Kind == ParameterKind.Number)
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
            string value;
            if (parameter.Kind == ParameterKind.String)
                value = (parameter as StringParameter).Value;
            else if (parameter.Kind == ParameterKind.Boolean)
                value = (parameter as BooleanParameter).Value.ToString();
            else if (parameter.Kind == ParameterKind.Number)
                value = (parameter as NumberParameter).Value.ToString(CultureInfo.InvariantCulture);
            else
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is not a value that can be converted to a string.");

            // Checks the type of the property, converts the value accordingly, and returns it
            if (propertyType == typeof(string))
                return value;
            if (propertyType == typeof(StringBuilder))
                return new StringBuilder(value);
            
            // Since the value could not be converted, an exception is thrown
            throw new InvalidOperationException("The parameter could not be converted, becaust the property is not assignable from string.");
        }

        #endregion
    }
}