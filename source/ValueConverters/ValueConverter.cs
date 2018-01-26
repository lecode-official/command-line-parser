
#region Using Directives

using System.Collections.Generic;

#endregion

namespace System.CommandLine.ValueConverters
{
    /// <summary>
    /// Represents a highly-customizable value conversion API, which can virtually convert any string to a meaningful C# object. It can be extended with new value converters.
    /// </summary>
    public static class ValueConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ValueConverter"/> statically. It initializes the built-in value converters.
        /// </summary>
        static ValueConverter() => ValueConverter.ResetValueConverters();

        #endregion

        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the value converters that can be used by <see cref="ValueConverter"/>.
        /// </summary>
        private static List<IValueConverter> valueConverters;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Converts the specified value to the specified type.
        /// </summary>
        /// <param name="value">The value that is to be converted to the specified type.</param>
        /// <param name="resultType">The type to which the specified value is to be converted.</param>
        /// <exception cref="InvalidOperationException">
        /// If no value converter was found that is capable of converting to the specified type or none of the value converters that were found could successfully convert the value, then an
        /// <see cref="InvalidOperationException"/> is thrown.
        /// </exception>
        /// <returns>Returns the converted value.</returns>
        public static object Convert(Type resultType, string value)
        {
            // Checks if the type is a collection type
            if (CollectionHelper.IsSupportedCollectionType(resultType))
            {
                Type elementType = CollectionHelper.GetCollectionElementType(resultType);
                object[] collectionValue = { ValueConverter.Convert(elementType, value) };
                return CollectionHelper.FromArray(resultType, collectionValue);
            }

            // Tries all the registered value converters one after another till a converter is found, that is able to convert the specified value to the specified type (the value converters
            // are ordered by priority, therefore they can be tried from first to last, if a value converter throws an error the search for a fitting value converter is continued)
            foreach (IValueConverter valueConverter in ValueConverter.valueConverters)
            {
                try
                {
                    if (resultType == typeof(object))
                    {
                        if (valueConverter.CanConvertFrom(value))
                            return valueConverter.Convert(resultType, value);
                    }
                    else
                    {
                        if (valueConverter.CanConvertTo(resultType))
                            return valueConverter.Convert(resultType, value);
                    }
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }

            // If this code is reached it means, that either the specified type is not supported by any of the value converters or the value could not be converted by any of them, in that case an exception is thrown
            throw new InvalidOperationException($"The value \"{value}\" cannot be converted to \"{resultType.Name}\".");
        }

        /// <summary>
        /// Adds a new value converter to the list of value converters used by <see cref="ValueConverter"/>. User-defined value converters have the highest priority and always tried before any built-in value converters
        /// are used. To restore the default behavior of <see cref="ValueConverter"/>, invoke <see cref="ResetValueConverters"/>.
        /// </summary>
        /// <param name="valueConverter">The value converter that is to be added.</param>
        public static void AddValueConverter(IValueConverter valueConverter)
        {
            if (valueConverter == null)
                throw new ArgumentNullException(nameof(valueConverter));
            ValueConverter.valueConverters.Insert(0, valueConverter);
        }

        /// <summary>
        /// Resets <see cref="ValueConverter"/> to its initial state. All user-defined value converters are removed and the default value-converters are re-initialized.
        /// </summary>
        public static void ResetValueConverters()
        {
            if (ValueConverter.valueConverters != null)
            {
                foreach (IValueConverter valueConverter in ValueConverter.valueConverters)
                {
                    IDisposable disposableValueConverter = valueConverter as IDisposable;
                    if (disposableValueConverter != null)
                        disposableValueConverter.Dispose();
                }
            }
            ValueConverter.valueConverters = new List<IValueConverter>
            {
                new IntegerConverter(),
                new FloatConverter(),
                new BooleanConverter(),
                new EnumerationConverter(),
                new StringConverter()
            };
        }

        #endregion
    }
}
