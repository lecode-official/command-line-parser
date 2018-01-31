
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
            Assert.False(booleanConverter.CanConvertTo(typeof(float)));
            Assert.False(booleanConverter.CanConvertTo(typeof(double)));
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

        /// <summary>
        /// Tests the value converter that can convert string to float values.
        /// </summary>
        [Fact]
        public void TestFloatConverter()
        {
            FloatConverter floatConverter = new FloatConverter();

            Assert.True(floatConverter.CanConvertTo(typeof(float)));
            Assert.True(floatConverter.CanConvertTo(typeof(double)));
            Assert.True(floatConverter.CanConvertTo(typeof(decimal)));

            Assert.False(floatConverter.CanConvertTo(typeof(string)));
            Assert.False(floatConverter.CanConvertTo(typeof(int)));
            Assert.False(floatConverter.CanConvertTo(typeof(Enum)));

            Assert.True(floatConverter.CanConvertFrom(" 10  "));
            Assert.True(floatConverter.CanConvertFrom("1.0"));
            Assert.True(floatConverter.CanConvertFrom(" .2"));
            Assert.True(floatConverter.CanConvertFrom(" 0      "));
            Assert.True(floatConverter.CanConvertFrom(" 1,000,000"));
            Assert.True(floatConverter.CanConvertFrom(" 123.456  "));
            Assert.True(floatConverter.CanConvertFrom(".123"));
            Assert.True(floatConverter.CanConvertFrom(" 1,234,567.89  "));
            Assert.True(floatConverter.CanConvertFrom("1e2"));
            Assert.True(floatConverter.CanConvertFrom("12E-3"));
            Assert.True(floatConverter.CanConvertFrom("1.2E4"));

            Assert.False(floatConverter.CanConvertFrom("abc"));
            Assert.False(floatConverter.CanConvertFrom(""));
            Assert.False(floatConverter.CanConvertFrom("   "));
            Assert.False(floatConverter.CanConvertFrom("."));
            Assert.False(floatConverter.CanConvertFrom("abc123"));
            Assert.False(floatConverter.CanConvertFrom("1 2 3"));
            Assert.False(floatConverter.CanConvertFrom("123e"));
            Assert.False(floatConverter.CanConvertFrom("e123"));

            Assert.Equal(1.0f, floatConverter.Convert(typeof(float), "1"));
            Assert.Equal(0.123d, floatConverter.Convert(typeof(double), " .123"));
            Assert.Equal(1234567m, floatConverter.Convert(typeof(decimal), "1,234,567  "));
            Assert.Equal(123.0m, floatConverter.Convert(typeof(object), "  123"));
            Assert.Equal(12.3f, floatConverter.Convert(typeof(float), "   1,2.3"));
            Assert.Equal(123.456d, floatConverter.Convert(typeof(double), "123.456  "));
            Assert.Equal(1.00001m, floatConverter.Convert(typeof(decimal), "00001.00001"));
            Assert.Equal(0.123m, floatConverter.Convert(typeof(object), ".123  "));
            Assert.Equal(1234567.89f, floatConverter.Convert(typeof(float), "1,234,567.89  "));
            Assert.Equal(123456789.0d, floatConverter.Convert(typeof(double), "  1,2,3,4,5,6,7,8,9"));
            Assert.Equal(0.0m, floatConverter.Convert(typeof(decimal), "  00000  "));
            Assert.Equal(1234.0m, floatConverter.Convert(typeof(object), "1,234"));
            Assert.Equal(12000.0f, floatConverter.Convert(typeof(float), "12e3"));
            Assert.Equal(0.01d, floatConverter.Convert(typeof(double), "10e-3"));
            Assert.Equal(10000000000000000.0m, floatConverter.Convert(typeof(decimal), "  10E+15  "));
            Assert.Equal(0.00035m, floatConverter.Convert(typeof(object), "3.5E-4"));

            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(float), "."));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(double), "122abc"));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(decimal), " 1 2 3 4 "));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(object), "123..123"));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(float), "e"));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(double), "1E"));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(decimal), " 3E"));
            Assert.Throws<InvalidOperationException>(() => floatConverter.Convert(typeof(object), "   3e4E-1   "));
        }

        #endregion
    }
}
