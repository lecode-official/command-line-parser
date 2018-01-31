
#region Using Directives

using System;
using System.Text;
using System.CommandLine.Arguments;
using System.CommandLine.ValueConverters;
using Xunit;

#endregion

namespace System.CommandLine.Parser.Tests
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

        #endregion
    }
}