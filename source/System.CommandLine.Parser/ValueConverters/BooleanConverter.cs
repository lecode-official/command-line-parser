
#region Using Directives

using System.Collections.Generic;
using System.Globalization;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a value converter, which is able to convert strings to booleans.
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the types that are supported by this value converter.
        /// </summary>
        private static readonly List<Type> supportedTypes = new List<Type> { typeof(bool) };

        /// <summary>
        /// Contains a conversion dictionary, which maps well-known string values to their respective boolean representation.
        /// </summary>
        private static readonly Dictionary<string, bool> booleanConversionMap = new Dictionary<string, bool>
        {
            ["TRUE"] = true,
            ["FALSE"] = false,
            ["YES"] = true,
            ["NO"] = false,
            ["ON"] = true,
            ["OFF"] = false,
        };

        #endregion

        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="type">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value)
        {
            // Checks if the value is one of the well-known strings that the value converter can convert to boolean
            if (BooleanConverter.booleanConversionMap.ContainsKey(value.Trim().ToUpperInvariant()))
                return true;

            // Checks if the value is numeric, in that case the value can also be converted
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                return true;

            // Since the value is neither one of the well-known strings, nor an integer value, the value cannot be converter
            return false;
        }

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type type) => BooleanConverter.supportedTypes.Contains(type);

        /// <summary>
        /// Converts the specified value to the specified destination type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified destination type.</param>
        /// <param name="type">The type to which the value is to be converted.</param>
        /// <exception cref="ArgumentNullException">If the value is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.
        /// <exception cref="InvalidOperationException">If the specified type is not supported or the value could not be converted, then an <see cref="InvalidOperationException"/> is thrown.
        /// <returns>Returns a new instance of the specified type, that contains the converted value.</returns>
        public object Convert(Type type, string value)
        {
            // Validates the argument
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            // Checks if the specified type is a supported type of the converter, if not, then an exception is thrown
            if (!BooleanConverter.supportedTypes.Contains(type))
                throw new InvalidOperationException($"The type \"{type.Name}\" is not supported.");

            // Tries to convert the specified value to the specified type
            if (BooleanConverter.booleanConversionMap.ContainsKey(value.Trim().ToUpperInvariant()))
                return BooleanConverter.booleanConversionMap[value.Trim().ToUpperInvariant()];
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integerValue))
                return integerValue != 0;

            // If this code is reached it means, that either the specified type is not supported or the value could not be converted, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{type.Name}\".");
        }

        #endregion
    }
}
