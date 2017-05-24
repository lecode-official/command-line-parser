
#region Using Directives

using System;
using System.CommandLine;
using Xunit;

#endregion

namespace System.CommandLine.Parser.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the camel casing routine.
    /// </summary>
    public class CamelCasingTests
    {
        /// <summary>
        /// Tests the camel-casing methods used for the positional arguments.
        /// </summary>
        [Fact]
        public void TestCamelCasing()
        {
            Assert.Equal("ThisIsATest", "tHiS iS a TeSt".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "this_is_a_test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "--this-is-a-test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", " this is a test ".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "T!h§i%s_i&s_a_t/e(s)t".ToCamelCasePropertyName());
            Assert.Equal("_45ThisIsATest123", "45this is a test123".ToCamelCasePropertyName());
        }
    }
}