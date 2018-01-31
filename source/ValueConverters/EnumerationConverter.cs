
#region Using Directives

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a value converter, which is able to convert strings to enumeration values.
    /// </summary>
    public class EnumerationConverter : IValueConverter
    {
        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value) => false;

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="resultType">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type resultType) => resultType.IsEnum;

        /// <summary>
        /// Converts the specified value to the specified destination type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified destination type.</param>
        /// <param name="resultType">The type to which the value is to be converted.</param>
        /// <exception cref="ArgumentNullException">If the type or the value is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.
        /// <exception cref="InvalidOperationException">If the specified type is not supported or the value could not be converted, then an <see cref="InvalidOperationException"/> is thrown.
        /// <returns>Returns a new instance of the specified type, that contains the converted value.</returns>
        public object Convert(Type resultType, string value)
        {
            // Validates the arguments
            if (resultType == null)
                throw new ArgumentNullException(nameof(resultType));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            // Checks if the result type is an enumeration, if not, then an exception is thrown
            if (!resultType.IsEnum)
                throw new InvalidOperationException($"The {resultType.Name} type is not an enumeration type.");

            // Tries to parse the enumeration by integer value
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int integerValue))
                return Enum.ToObject(resultType, integerValue);

            // Tries to parse the enumeration by name
            if (!Enum.GetNames(resultType).Contains(value))
                throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{resultType.Name}\".");
            return Enum.Parse(resultType, value);
        }

        #endregion
    }
}
