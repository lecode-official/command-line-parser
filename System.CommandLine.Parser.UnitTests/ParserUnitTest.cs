
#region Using Directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace System.CommandLine.Parser.UnitTests
{
    /// <summary>
    /// Represents a unit testing class, which tests the actual command line parser, which turns the command line parameters into typed .NET objects.
    /// </summary>
    [TestClass]
    public class ParserUnitTest
    {
        #region Private Methods

        /// <summary>
        /// Validates that the parse output against the specified expected output.
        /// </summary>
        /// <param name="parameters">The parameters, which are the result of the parsing process.</param>
        /// <param name="expectedParameters">The parameters, which are the expected output of the parsing process.</param>
        private void ValidateParseOutput(IEnumerable<Parameter> parameters, IEnumerable<Parameter> expectedParameters)
        {
            // Validates that the amount of parameters in the parameter bag and the expected parameter bag are the same
            Assert.AreEqual(parameters.Count(), expectedParameters.Count());
            if (parameters.Count() != expectedParameters.Count())
                return;

            // Cycles over all parameters in the parameter bag and validates them against the expected parameters
            for (int i = 0; i < parameters.Count(); i++)
            {
                // Gets the two parameter at the current position
                Parameter parameter = parameters.ElementAt(i);
                Parameter expectedParameter = expectedParameters.ElementAt(i);

                // Validates that the parameter has the same type as the expected parameter
                Assert.IsInstanceOfType(parameter, expectedParameter.GetType());
                if (parameter.GetType() != expectedParameter.GetType())
                    continue;

                // Validates that the parameter has the same name as the expected parameter
                Assert.AreEqual(parameter.Name, expectedParameter.Name);

                // Validates whether both the parameter and the expected parameter are default parameters or both are not
                Assert.AreEqual(parameter.IsDefaultParameter, expectedParameter.IsDefaultParameter);

                // Checks if the parameter is a simple data type, if so its value is validated againts the expected parameter
                BooleanParameter booleanParameter = parameter as BooleanParameter;
                if (booleanParameter != null)
                    Assert.AreEqual(booleanParameter.Value, (expectedParameter as BooleanParameter).Value);
                NumberParameter numberParameter = parameter as NumberParameter;
                if (numberParameter != null)
                    Assert.AreEqual(numberParameter.Value, (expectedParameter as NumberParameter).Value);
                StringParameter stringParameter = parameter as StringParameter;
                if (stringParameter != null)
                    Assert.AreEqual(stringParameter.Value, (expectedParameter as StringParameter).Value);

                // Checks if the parameter is of type array, if so then its contents are validated recursively
                ArrayParameter arrayParameter = parameter as ArrayParameter;
                if (arrayParameter != null)
                    this.ValidateParseOutput(arrayParameter.Value, (expectedParameter as ArrayParameter).Value);
            }
        }

        #endregion

        #region General Test Methods

        /// <summary>
        /// Tests how the parser handles empty command line parameters.
        /// </summary>
        [TestMethod]
        public void EmptyCommandLineParametersTest()
        {
            // Parses empty command line parameters
            ParameterBag parameterBag = Parser.Parse(string.Empty);

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>());
        }

        #endregion

        #region Default Parameter Test Methods

        /// <summary>
        /// Tests how the parser handles a single default parameter.
        /// </summary>
        [TestMethod]
        public void SingleDefaultParameterTest()
        {
            // Parses a single default command line parameter
            ParameterBag parameterBag = Parser.Parse("abcXYZ");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new DefaultParameter { Value = "abcXYZ" }
            });
        }

        /// <summary>
        /// Tests how the parser handles multiple default parameters.
        /// </summary>
        [TestMethod]
        public void MutlipleDefaultParameterTest()
        {
            // Parses multiple default command line parameters
            ParameterBag parameterBag = Parser.Parse("abc \"123 456\" XYZ \"789 0\"");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new DefaultParameter { Value = "abc" },
                new DefaultParameter { Value = "123 456" },
                new DefaultParameter { Value = "XYZ" },
                new DefaultParameter { Value = "789 0" }
            });
        }

        #endregion

        #region Parameter Test Methods

        /// <summary>
        /// Tests how the parser handles Windows style switches.
        /// </summary>
        [TestMethod]
        public void WindowsStyleSwitchTest()
        {
            // Parses a Windows style switch
            ParameterBag parameterBag = Parser.Parse("/Switch");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new BooleanParameter
                {
                    Name = "Switch",
                    Value = true
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles Windows style parameters.
        /// </summary>
        [TestMethod]
        public void WindowsStyleParameterTest()
        {
            // Parses a Windows style parameter
            ParameterBag parameterBag = Parser.Parse("/Parameter:123");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "Parameter",
                    Value = 123.0d
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style switches.
        /// </summary>
        [TestMethod]
        public void UnixStyleSwitchTest()
        {
            // Parses a UNIX style switch
            ParameterBag parameterBag = Parser.Parse("--Switch");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new BooleanParameter
                {
                    Name = "Switch",
                    Value = true
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style parameters.
        /// </summary>
        [TestMethod]
        public void UnixStyleParameterTest()
        {
            // Parses a UNIX style parameter
            ParameterBag parameterBag = Parser.Parse("--Parameter=\"abc XYZ\"");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new StringParameter
                {
                    Name = "Parameter",
                    Value = "abc XYZ"
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style flagged switches.
        /// </summary>
        [TestMethod]
        public void UnixStyleFlaggedSwitchesTest()
        {
            // Parses a UNIX style flagged switch
            ParameterBag parameterBag = Parser.Parse("-sUtZ");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new BooleanParameter
                {
                    Name = "s",
                    Value = true
                },
                new BooleanParameter
                {
                    Name = "U",
                    Value = true
                },
                new BooleanParameter
                {
                    Name = "t",
                    Value = true
                },
                new BooleanParameter
                {
                    Name = "Z",
                    Value = true
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles multiple parameters.
        /// </summary>
        [TestMethod]
        public void MultipleParameterTest()
        {
            // Parses a multiple parameter
            ParameterBag parameterBag = Parser.Parse("/on /key:value --auto --parameter=123 -aFl");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new BooleanParameter
                {
                    Name = "on",
                    Value = true
                },
                new StringParameter
                {
                    Name = "key",
                    Value = "value"
                },
                new BooleanParameter
                {
                    Name = "auto",
                    Value = true
                },
                new NumberParameter
                {
                    Name = "parameter",
                    Value = 123.0d
                },
                new BooleanParameter
                {
                    Name = "a",
                    Value = true
                },
                new BooleanParameter
                {
                    Name = "F",
                    Value = true
                },
                new BooleanParameter
                {
                    Name = "l",
                    Value = true
                }
            });
        }

        #endregion

        #region Mixed Default Parameter & Parameter Test Methods

        /// <summary>
        /// Tests how the parser handles mixing of default parameters and parameters.
        /// </summary>
        [TestMethod]
        public void MixedDefaultParameterAndParameterTest()
        {
            // Parses multiple parameters
            ParameterBag parameterBag = Parser.Parse("\"C:\\Users\\name\\Downloads\" /key:value --auto");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new DefaultParameter { Value = "C:\\Users\name\\Downloads" },
                new StringParameter
                {
                    Name = "key",
                    Value = "value"
                },
                new BooleanParameter
                {
                    Name = "auto",
                    Value = true
                }
            });
        }

        #endregion
    }
}