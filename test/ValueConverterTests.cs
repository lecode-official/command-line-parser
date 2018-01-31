
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

        /// <summary>
        /// Tests the value converter that can convert string to integer values.
        /// </summary>
        [Fact]
        public void TestIntegerConverter()
        {
            IntegerConverter integerConverter = new IntegerConverter();

            Assert.True(integerConverter.CanConvertTo(typeof(byte)));
            Assert.True(integerConverter.CanConvertTo(typeof(sbyte)));
            Assert.True(integerConverter.CanConvertTo(typeof(short)));
            Assert.True(integerConverter.CanConvertTo(typeof(ushort)));
            Assert.True(integerConverter.CanConvertTo(typeof(int)));
            Assert.True(integerConverter.CanConvertTo(typeof(uint)));
            Assert.True(integerConverter.CanConvertTo(typeof(long)));
            Assert.True(integerConverter.CanConvertTo(typeof(ulong)));

            Assert.False(integerConverter.CanConvertTo(typeof(string)));
            Assert.False(integerConverter.CanConvertTo(typeof(float)));
            Assert.False(integerConverter.CanConvertTo(typeof(double)));
            Assert.False(integerConverter.CanConvertTo(typeof(decimal)));
            Assert.False(integerConverter.CanConvertTo(typeof(Enum)));

            Assert.True(integerConverter.CanConvertFrom(" 10  "));
            Assert.True(integerConverter.CanConvertFrom("123"));
            Assert.True(integerConverter.CanConvertFrom("1,2,3,4   "));
            Assert.True(integerConverter.CanConvertFrom("   1,234,567  "));
            Assert.True(integerConverter.CanConvertFrom(" 000,000"));
            Assert.True(integerConverter.CanConvertFrom(" 3e7  "));
            Assert.True(integerConverter.CanConvertFrom("500e-2"));
            Assert.True(integerConverter.CanConvertFrom("    6E4  "));
            Assert.True(integerConverter.CanConvertFrom(" 2E+5"));

            Assert.False(integerConverter.CanConvertFrom("abc"));
            Assert.False(integerConverter.CanConvertFrom(""));
            Assert.False(integerConverter.CanConvertFrom("   "));
            Assert.False(integerConverter.CanConvertFrom("."));
            Assert.False(integerConverter.CanConvertFrom("  abc123"));
            Assert.False(integerConverter.CanConvertFrom("0x   "));
            Assert.False(integerConverter.CanConvertFrom("  e4  "));
            Assert.False(integerConverter.CanConvertFrom("23e  "));
            Assert.False(integerConverter.CanConvertFrom(" 1 2 3"));
            Assert.False(integerConverter.CanConvertFrom(" 1E-4 "));
            Assert.False(integerConverter.CanConvertFrom(" --4"));

            Assert.Equal((byte)1, integerConverter.Convert(typeof(byte), "1"));
            Assert.Equal((sbyte)123, integerConverter.Convert(typeof(sbyte), " 123"));
            Assert.Equal((short)-23, integerConverter.Convert(typeof(short), "-23    "));
            Assert.Equal((ushort)5000, integerConverter.Convert(typeof(ushort), "   5,0,0,0   "));
            Assert.Equal(-20000, integerConverter.Convert(typeof(int), " -2e4 "));
            Assert.Equal(2300000u, integerConverter.Convert(typeof(uint), "23e5"));
            Assert.Equal(1234567890L, integerConverter.Convert(typeof(long), "1,234,567,890  "));
            Assert.Equal(123456789UL, integerConverter.Convert(typeof(ulong), "  1,2,3,4,5,6,7,8,9"));
            Assert.Equal(123400L, integerConverter.Convert(typeof(object), "    1,234E2    "));

            Assert.Throws<InvalidOperationException>(() => integerConverter.Convert(typeof(byte), ","));
            Assert.Throws<InvalidOperationException>(() => integerConverter.Convert(typeof(sbyte), "256"));
            Assert.Throws<InvalidOperationException>(() => integerConverter.Convert(typeof(short), "1234567890"));
            Assert.Throws<InvalidOperationException>(() => integerConverter.Convert(typeof(object), "123e-2"));
        }

        #endregion
    }
}
