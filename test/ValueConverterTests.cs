
#region Using Directives

using System;
using System.CommandLine.Arguments;
using System.CommandLine.ValueConverters;
using Xunit;

#endregion

namespace System.CommandLine.Parser.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the value converters.
    /// </summary>
    public class ValueConverterTests
    {
        #region Unit Tests

        /// <summary>
        /// Tests the value converter that can convert string to boolean values.
        /// </summary>
        [Fact]
        public void TestBooleanConverter()
        {
            BooleanConverter booleanConverter = new BooleanConverter();

            Assert.True(booleanConverter.CanConvertTo(typeof(bool)));
            Assert.True(booleanConverter.CanConvertTo(typeof(Boolean)));

            Assert.False(booleanConverter.CanConvertTo(typeof(string)));
            Assert.False(booleanConverter.CanConvertTo(typeof(int)));

            Assert.True(booleanConverter.CanConvertFrom(" oN   "));
            Assert.True(booleanConverter.CanConvertFrom("OFF  "));
            Assert.True(booleanConverter.CanConvertFrom(" YeS"));
            Assert.True(booleanConverter.CanConvertFrom("No"));
            Assert.True(booleanConverter.CanConvertFrom("true"));
            Assert.True(booleanConverter.CanConvertFrom("FALSE"));
            Assert.True(booleanConverter.CanConvertFrom("10"));

            Assert.False(booleanConverter.CanConvertFrom("abc"));
            Assert.False(booleanConverter.CanConvertFrom("No!"));

            Assert.Equal(true, booleanConverter.Convert(typeof(bool), "TRUE"));
            Assert.Equal(false, booleanConverter.Convert(typeof(object), "fAlSe"));
            Assert.Equal(true, booleanConverter.Convert(typeof(bool), "on"));
            Assert.Equal(false, booleanConverter.Convert(typeof(object), "oFF"));
            Assert.Equal(true, booleanConverter.Convert(typeof(bool), "yeS"));
            Assert.Equal(false, booleanConverter.Convert(typeof(object), "No"));
            Assert.Equal(true, booleanConverter.Convert(typeof(bool), "-10"));
            Assert.Equal(true, booleanConverter.Convert(typeof(bool), "123"));
            Assert.Equal(false, booleanConverter.Convert(typeof(object), "0"));

            Assert.Throws<InvalidOperationException>(() => booleanConverter.Convert(typeof(object), "0.0"));
            Assert.Throws<InvalidOperationException>(() => booleanConverter.Convert(typeof(object), "XYZ"));
        }

        #endregion
    }
}
