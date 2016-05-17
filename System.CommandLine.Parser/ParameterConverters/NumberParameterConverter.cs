
#region Using Directives

using System.CommandLine.Parser.Parameters;
using System.Globalization;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert parameters into CLR number types.
    /// </summary>
    public class NumberParameterConverter : IParameterConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains the american culture info, which is used for number parsing.
        /// </summary>
        private static CultureInfo americanCultureInfo = new CultureInfo("en-US");

        #endregion

        #region IParameterConverter Implementation

        /// <summary>
        /// Dertermines whether the specified parameter can be converted into the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property into which the parameter is to be converted.</param>
        /// <param name="parameter">The parameters that is to be converted into the specified type.</param>
        /// <returns>Returns <c>true</c> if the specified parameter can be converted into the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvert(Type propertyType, Parameter parameter)
        {
            // Checks if the property type is supported (which is when the property type is a CLR number type), if not then false is returned
            if (propertyType != typeof(decimal) &&
                propertyType != typeof(double) &&
                propertyType != typeof(float) &&
                propertyType != typeof(long) &&
                propertyType != typeof(int) &&
                propertyType != typeof(short) &&
                propertyType != typeof(byte))
            {
                return false;
            }

            // Checks if the type of the parameter is string, if so then the parameter can be converted, if the string contains a valid number
            if (parameter.Kind == ParameterKind.String)
            {
                decimal value;
                if (decimal.TryParse((parameter as StringParameter).Value, NumberStyles.Number, NumberParameterConverter.americanCultureInfo, out value))
                    return true;
                else
                    return false;
            }

            // Checks if the type of the parameter is one of the supported types (numbers and booleans are supported)
            return parameter.Kind == ParameterKind.Number || parameter.Kind == ParameterKind.Boolean;
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
            decimal value;
            if (parameter.Kind == ParameterKind.Number)
            {
                value = (parameter as NumberParameter).Value;
            }
            else if (parameter.Kind == ParameterKind.String)
            {
                if (decimal.TryParse((parameter as StringParameter).Value, NumberStyles.Number, NumberParameterConverter.americanCultureInfo, out value))
                    throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is a string, which could not be converted into a number.");
            }
            else if (parameter.Kind == ParameterKind.Boolean)
            {
                value = (parameter as BooleanParameter).Value ? 1 : 0;
            }
            else
            {
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is not a value that can be converted into a number.");
            }

            // Converts the retrieved value into the destination type
            if (propertyType == typeof(decimal))
                return System.Convert.ToDecimal(value);
            if (propertyType == typeof(double))
                return System.Convert.ToDouble(value);
            if (propertyType == typeof(float))
                return System.Convert.ToSingle(value);
            if (propertyType == typeof(long))
                return System.Convert.ToInt64(value);
            if (propertyType == typeof(int))
                return System.Convert.ToInt32(value);
            if (propertyType == typeof(short))
                return System.Convert.ToInt16(value);
            if (propertyType == typeof(byte))
                return System.Convert.ToByte(value);

            // Since the value could not be converted, an exception is thrown
            throw new InvalidOperationException("The parameter could not be converted, because the property is not assignable from any of the supported number types.");
        }

        #endregion
    }
}