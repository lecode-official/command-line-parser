
#region Using Directives

using System.Collections.Generic;
using System.Globalization;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a value converter, which is able to convert strings to floats.
    /// </summary>
    public class FloatConverter : IValueConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the types that are supported by this value converter.
        /// </summary>
        private static readonly List<Type> supportedTypes = new List<Type> { typeof(float), typeof(double), typeof(decimal) };

        /// <summary>
        /// Contains the number style that is used to parse the floating point literals.
        /// </summary>
        private static readonly NumberStyles numberStyles = NumberStyles.Float | NumberStyles.AllowThousands;

        #endregion

        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="type">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value) => decimal.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out _);

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type type) => FloatConverter.supportedTypes.Contains(type);

        /// <summary>
        /// Converts the specified value to the specified destination type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified destination type.</param>
        /// <param name="type">The type to which the value is to be converted.</param>
        /// <exception cref="ArgumentNullException">If the type or the value is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.
        /// <exception cref="InvalidOperationException">If the specified type is not supported or the value could not be converted, then an <see cref="InvalidOperationException"/> is thrown.
        /// <returns>Returns a new instance of the specified type, that contains the converted value.</returns>
        public object Convert(Type type, string value)
        {
            // Validates the arguments
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            // Tries to convert the specified value to the specified type
            try
            {
                if (type == typeof(float))
                    return float.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(double))
                    return double.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(decimal))
                    return decimal.Parse(value, numberStyles, CultureInfo.InvariantCulture);
            }
            catch (FormatException) {}

            // If this code is reached it means, that either the specified type is not supported or the value could not be converted, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{type.Name}\".");
        }

        #endregion
    }
}
