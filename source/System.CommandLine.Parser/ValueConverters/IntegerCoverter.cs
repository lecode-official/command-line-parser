
#region Using Directives

using System.Collections.Generic;
using System.Globalization;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a value converter, which is able to convert strings to integers.
    /// </summary>
    public class IntegerConverter : IValueConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the types that are supported by this value converter.
        /// </summary>
        private static List<Type> supportedTypes = new List<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };

        /// <summary>
        /// Contains the number style that is used to parse the integer literals.
        /// </summary>
        private static readonly NumberStyles numberStyles = NumberStyles.Integer | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowHexSpecifier;

        #endregion

        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="type">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value) => long.TryParse(
            value,
            numberStyles,
            CultureInfo.InvariantCulture,
            out _
        );

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type type) => IntegerConverter.supportedTypes.Contains(type);

        /// <summary>
        /// Converts the specified value to the specified destination type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified destination type.</param>
        /// <param name="type">The type to which the value is to be converted.</param>
        /// <exception cref="ArgumentNullException">If the value is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.
        /// <returns>Returns a new instance of the specified type, that contains the converted value.</returns>
        public object Convert(Type type, string value)
        {
            // Validates the argument
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            // Tries to convert the specified value to the specified type
            try
            {
                if (type == typeof(byte))
                    return byte.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(sbyte))
                    return sbyte.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(short))
                    return short.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(ushort))
                    return ushort.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(int))
                    return int.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(uint))
                    return uint.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(long))
                    return long.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                if (type == typeof(ulong))
                    return ulong.Parse(value, numberStyles, CultureInfo.InvariantCulture);
            }
            catch (FormatException) {}

            // If this code is reached it means, that either the specified type is not supported or the value could not be converted, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{type.Name}\".");
        }

        #endregion
    }
}
