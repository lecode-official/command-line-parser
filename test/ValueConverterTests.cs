
#region Using Directives

using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.CommandLine.Arguments;
using System.CommandLine.ValueConverters;
using Xunit;

#endregion

namespace System.CommandLine.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the <see ref="ValueConverter"/> class.
    /// </summary>
    public class ValueConverterTests
    {
        #region Unit Tests

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles the conversion of boolean values.
        /// </summary>
        [Fact]
        public void TestBooleanValueConversion()
        {
            Assert.Equal(true, ValueConverter.Convert(typeof(bool), "true"));
            Assert.Equal(false, ValueConverter.Convert(typeof(bool), "false"));
            Assert.Equal(true, ValueConverter.Convert(typeof(bool), "on"));
            Assert.Equal(false, ValueConverter.Convert(typeof(bool), "off"));
            Assert.Equal(true, ValueConverter.Convert(typeof(bool), "yes"));
            Assert.Equal(false, ValueConverter.Convert(typeof(bool), "no"));
            Assert.Equal(true, ValueConverter.Convert(typeof(bool), "1"));
            Assert.Equal(false, ValueConverter.Convert(typeof(bool), "0"));
        }

        /// <summary>
        /// Represents an enumeration used for testing the conversion of enumeration values.
        /// </summary>
        [Flags]
        private enum Color { Red = 1, Green = 2, Blue = 4 }

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles the conversion of enumeration values.
        /// </summary>
        [Fact]
        public void TestEnumerationValueConversion()
        {
            Assert.Equal(Color.Red, ValueConverter.Convert(typeof(Color), "Red"));
            Assert.Equal(Color.Green, ValueConverter.Convert(typeof(Color), "Green"));
            Assert.Equal(Color.Blue, ValueConverter.Convert(typeof(Color), "Blue"));
            Assert.Equal(Color.Red | Color.Blue, ValueConverter.Convert(typeof(Color), "5"));

            Assert.Equal(DayOfWeek.Monday, ValueConverter.Convert(typeof(DayOfWeek), "Monday"));
            Assert.Equal(DayOfWeek.Tuesday, ValueConverter.Convert(typeof(DayOfWeek), "Tuesday"));
            Assert.Equal(DayOfWeek.Wednesday, ValueConverter.Convert(typeof(DayOfWeek), "Wednesday"));
            Assert.Equal(DayOfWeek.Thursday, ValueConverter.Convert(typeof(DayOfWeek), "Thursday"));
            Assert.Equal(DayOfWeek.Friday, ValueConverter.Convert(typeof(DayOfWeek), "Friday"));
            Assert.Equal(DayOfWeek.Saturday, ValueConverter.Convert(typeof(DayOfWeek), "Saturday"));
            Assert.Equal(DayOfWeek.Sunday, ValueConverter.Convert(typeof(DayOfWeek), "Sunday"));
        }

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles the conversion of integer values.
        /// </summary>
        [Fact]
        public void TestIntegerValueConversion()
        {
            Assert.Equal((byte)1, ValueConverter.Convert(typeof(byte), "1"));
            Assert.Equal((sbyte)123, ValueConverter.Convert(typeof(sbyte), "123"));
            Assert.Equal((short)-23, ValueConverter.Convert(typeof(short), "-23"));
            Assert.Equal((ushort)5000, ValueConverter.Convert(typeof(ushort), "5,0,0,0"));
            Assert.Equal(-20000, ValueConverter.Convert(typeof(int), "-2e4"));
            Assert.Equal(2300000u, ValueConverter.Convert(typeof(uint), "23e5"));
            Assert.Equal(1234567890L, ValueConverter.Convert(typeof(long), "1,234,567,890"));
            Assert.Equal(123456789UL, ValueConverter.Convert(typeof(ulong), "1,2,3,4,5,6,7,8,9"));
            Assert.Equal(123400L, ValueConverter.Convert(typeof(object), "1,234E2"));

            Assert.Equal((byte)3, ValueConverter.Convert(typeof(byte), "+0x3"));
            Assert.Equal((sbyte)123, ValueConverter.Convert(typeof(sbyte), "0X7B"));
            Assert.Equal((short)-35, ValueConverter.Convert(typeof(short), "-0x23"));
            Assert.Equal((ushort)2748, ValueConverter.Convert(typeof(ushort), "+0xABC"));
            Assert.Equal(-11259375, ValueConverter.Convert(typeof(int), "-0xAbCdEf"));
            Assert.Equal(65535u, ValueConverter.Convert(typeof(uint), "0XFFFF"));
            Assert.Equal(-78187493520L, ValueConverter.Convert(typeof(long), "-0x1234567890"));
            Assert.Equal(0UL, ValueConverter.Convert(typeof(ulong), "0x0"));
            Assert.Equal(3735928559L, ValueConverter.Convert(typeof(object), "0XDEADBEEF"));
        }

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles the conversion of float values.
        /// </summary>
        [Fact]
        public void TestFloatValueConversion()
        {
            Assert.Equal(1.0f, ValueConverter.Convert(typeof(float), "1"));
            Assert.Equal(0.123d, ValueConverter.Convert(typeof(double), ".123"));
            Assert.Equal(1234567m, ValueConverter.Convert(typeof(decimal), "1,234,567"));
            Assert.Equal(123L, ValueConverter.Convert(typeof(object), "123"));
            Assert.Equal(12.3f, ValueConverter.Convert(typeof(float), "1,2.3"));
            Assert.Equal(123.456d, ValueConverter.Convert(typeof(double), "123.456"));
            Assert.Equal(1.00001m, ValueConverter.Convert(typeof(decimal), "00001.00001"));
            Assert.Equal(0.123m, ValueConverter.Convert(typeof(object), ".123"));
            Assert.Equal(1234567.89f, ValueConverter.Convert(typeof(float), "1,234,567.89"));
            Assert.Equal(123456789.0d, ValueConverter.Convert(typeof(double), "1,2,3,4,5,6,7,8,9"));
            Assert.Equal(0.0m, ValueConverter.Convert(typeof(decimal), "00000"));
            Assert.Equal(1234L, ValueConverter.Convert(typeof(object), "1,234"));
            Assert.Equal(12000.0f, ValueConverter.Convert(typeof(float), "12e3"));
            Assert.Equal(0.01d, ValueConverter.Convert(typeof(double), "10e-3"));
            Assert.Equal(10000000000000000.0m, ValueConverter.Convert(typeof(decimal), "10E+15"));
            Assert.Equal(0.00035m, ValueConverter.Convert(typeof(object), "3.5E-4"));
        }

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles the conversion of any values to collections.
        /// </summary>
        [Fact]
        public void TestCollectionConversion()
        {
            Assert.Equal(new Color[] { Color.Red }, ValueConverter.Convert(typeof(Color[]), "Red"));
            Assert.Equal(new Color[] { Color.Red | Color.Blue }, ValueConverter.Convert(typeof(Color[]), "5"));
            Assert.Equal(new int[] { -255 }, ValueConverter.Convert(typeof(int[]), "-0xff"));

            Assert.Equal(new List<int> { 1 }, ValueConverter.Convert(typeof(List<int>), "1"));
            Assert.Equal(new List<object> { false }, ValueConverter.Convert(typeof(List<object>), "False"));
        }

        /// <summary>
        /// Represents a very simple value converter for IP addresses, which is used to test custom value converters.
        /// </summary>
        private class IPAddressConverter : IValueConverter
        {
            public bool CanConvertFrom(string value) => IPAddress.TryParse(value, out _);
            public bool CanConvertTo(Type resultType) => resultType == typeof(IPAddress);
            public object Convert(Type resultType, string value) => IPAddress.Parse(value);
        }

        /// <summary>
        /// Tests how the <see ref="ValueConverter"/> handles custom value converters.
        /// </summary>
        [Fact]
        public void TestCustomValueConverters()
        {
            ValueConverter.AddValueConverter(new IPAddressConverter());
            Assert.Equal(IPAddress.Loopback, ValueConverter.Convert(typeof(IPAddress), "127.0.0.1"));
            Assert.Equal(IPAddress.IPv6Loopback, ValueConverter.Convert(typeof(IPAddress), "::1"));
            Assert.Equal(new IPAddress(new byte[] { 0, 0, 0, 0 }), ValueConverter.Convert(typeof(object), "0.0.0.0"));
            ValueConverter.ResetValueConverters();
        }

        #endregion
    }
}