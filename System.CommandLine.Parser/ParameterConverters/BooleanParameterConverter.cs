
#region Using Directives

using System.CommandLine.Parser.Parameters;

#endregion

namespace System.CommandLine.Parser.ParameterConverters
{
    /// <summary>
    /// Represents a parameter converter, which is able to convert parameters into boolean types.
    /// </summary>
    public class BooleanParameterConverter : IParameterConverter
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
            if (propertyType != typeof(bool))
                return false;

            // Checks if the parameter belongs to one of the supported types (boolean, and number)
            if (parameter.Kind == ParameterKind.Boolean || parameter.Kind == ParameterKind.Number)
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
            bool value;
            if (parameter.Kind == ParameterKind.Boolean)
                value = (parameter as BooleanParameter).Value;
            else if (parameter.Kind == ParameterKind.Number)
                value = (parameter as NumberParameter).Value != 0;
            else
                throw new InvalidOperationException("The parameter could not be converted, because the command line parameter is not a value that can be converted into a boolean.");

            // Checks the type of the property, converts the value accordingly, and returns it
            if (propertyType == typeof(bool))
                return value;

            // Since the value could not be converted, an exception is thrown
            throw new InvalidOperationException("The parameter could not be converted, because the property is not assignable from boolean.");
        }

        #endregion
    }
}