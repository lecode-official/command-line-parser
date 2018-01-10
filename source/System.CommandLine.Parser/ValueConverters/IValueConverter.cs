
namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents an interface that all value converter extensions for the <see cref="ValueConverter"/> must implement.
    /// </summary>
    public interface IValueConverter
    {
        #region Methods

        /// <summary>
        /// Determines whether this value converter is able to convert a string to the specified type.
        /// </summary>
        /// <typeparam name="T">The type which is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter can convert to the specified type and <c>false</c> otherwise.</returns>
        bool CanConvertTo<T>();

        /// <summary>
        /// Determines whether this value converter is able to convert the specified string.
        /// </summary>
        /// <param name="value">The value that is to be tested.</param>
        /// <returns>Returns <c>true</c> if the value converter is able to convert the specified type and <c>false</c> otherwise.</returns>
        bool CanConvertFrom(string value);

        /// <summary>
        /// Converts the specified value to the specified destination type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified destination type.</param>
        /// <typeparam name="T">The type to which the value is to be converted.</param>
        /// <returns>Returns a new instance of the specified type, that contains the converted value.</returns>
        T Convert<T>(string value);

        #endregion
    }
}
