
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

        #endregion
    }
}