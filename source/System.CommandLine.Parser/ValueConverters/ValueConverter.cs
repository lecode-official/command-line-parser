
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
                new BooleanConverter()
            };
        }

        #endregion
    }
}
