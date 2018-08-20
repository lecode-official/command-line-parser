
#region Using Directives

using System;
using System.CommandLine.Arguments;
using Xunit;

#endregion

namespace System.CommandLine.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the camel casing routine.
    /// </summary>
    public class CamelCasingTests
    {
        #region Unit Tests

        /// <summary>
        /// Tests the camel-casing methods used for the named arguments.
        /// </summary>
        [Fact]
        public void TestCamelCasing()
        {
            Assert.Equal("ThisIsATest", "This is a Test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "this_is_a_test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "ThisIsATest".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "--this-is-a-test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "/this_is_a_test".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", " this is a test ".ToCamelCasePropertyName());
            Assert.Equal("ThisIsATest", "T!h§i%s_i&s_a_t/e(s)t".ToCamelCasePropertyName());
            Assert.Equal("_45ThisIsATest123", "45this is a test123".ToCamelCasePropertyName());
        }

        #endregion
    }
}
