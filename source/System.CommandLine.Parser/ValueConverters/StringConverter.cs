
#region Using Directives

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a value converter, which is able to convert strings to strings.
    /// </summary>
    public class StringConverter : IValueConverter
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the types that are supported by this value converter.
        /// </summary>
        private static readonly List<Type> supportedTypes = new List<Type> { typeof(string), typeof(StringBuilder) };

        #endregion

        #region IValueConverter Implementation

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <param name="type">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertFrom(string value) => true;

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        public bool CanConvertTo(Type type) => StringConverter.supportedTypes.Contains(type);

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
            if (type == typeof(StringBuilder))
                return new StringBuilder(value);
            if (type == typeof(string))
                return value;

            // If this code is reached it means, that either the specified type is not supported or the value could not be converted, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{type.Name}\".");
        }

        #endregion
    }
}
