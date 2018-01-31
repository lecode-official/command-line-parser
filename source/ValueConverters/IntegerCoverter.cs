
#region Using Directives

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

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
        private static readonly NumberStyles numberStyles = NumberStyles.Integer | NumberStyles.AllowThousands | NumberStyles.AllowExponent;

        #endregion

        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value)
        {
            Regex hexNumberRegex = new Regex(@"^[ ]*[\+\-]?(0x)", RegexOptions.IgnoreCase);
            if (hexNumberRegex.IsMatch(value))
                return long.TryParse(hexNumberRegex.Replace(value, string.Empty, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
            return long.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="resultType">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type resultType) => IntegerConverter.supportedTypes.Contains(resultType);

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

            // Tries to convert the specified value to the specified type
            try
            {
                Regex hexNumberRegex = new Regex(@"^[ ]*[\+\-]?(0x)", RegexOptions.IgnoreCase);
                if (hexNumberRegex.IsMatch(value))
                {
                    int sign = value.Contains("-") ? -1 : 1;
                    value = hexNumberRegex.Replace(value, string.Empty, 1);
                    if (resultType == typeof(byte) && sign == 1)
                        return byte.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (resultType == typeof(sbyte))
                        return (sbyte)(sbyte.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) * sign);
                    if (resultType == typeof(short))
                        return (short)(short.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) * sign);
                    if (resultType == typeof(ushort) && sign == 1)
                        return ushort.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (resultType == typeof(int))
                        return (int)(int.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) * sign);
                    if (resultType == typeof(uint) && sign == 1)
                        return uint.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (resultType == typeof(long) || resultType == typeof(object))
                        return (long)(long.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) * sign);
                    if (resultType == typeof(ulong) && sign == 1)
                        return ulong.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }
                else
                {
                    if (resultType == typeof(byte))
                        return byte.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(sbyte))
                        return sbyte.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(short))
                        return short.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(ushort))
                        return ushort.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(int))
                        return int.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(uint))
                        return uint.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(long) || resultType == typeof(object))
                        return long.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                    if (resultType == typeof(ulong))
                        return ulong.Parse(value, numberStyles, CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException) {}
            catch (OverflowException) {}

            // If this code is reached it means, that either the specified type is not supported or the value could not be converted, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{resultType.Name}\".");
        }

        #endregion
    }
}
