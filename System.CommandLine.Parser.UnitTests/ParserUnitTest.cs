
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

        #region Data Type Test Methods

        /// <summary>
        /// Tests how the parser handles boolean values.
        /// </summary>
        [TestMethod]
        public void BooleanDataTypeTest()
        {
            // Parses a boolean value
            ParameterBag parameterBag = Parser.Parse("/first:false --second=true");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new BooleanParameter
                {
                    Name = "first",
                    Value = false
                },
                new BooleanParameter
                {
                    Name = "second",
                    Value = true
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles numbers.
        /// </summary>
        [TestMethod]
        public void NumberDataTypeTest()
        {
            // Parses a positive integer and validates that the parsed parameters are correct
            ParameterBag parameterBag = Parser.Parse("/parameter=123");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = 123.0d
                }
            });

            // Parses a negative integer and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("--parameter:-123");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = -123.0d
                }
            });

            // Parses a positive floating point number and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("/parameter 123.456");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = 123.456d
                }
            });

            // Parses a negative floating point number and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("--parameter -123.456");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = -123.456d
                }
            });

            // Parses a positive floating point number with no digits before the decimal point and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("/parameter .123");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = 0.123d
                }
            });

            // Parses a negative floating point number with no digits before the decimal point and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("--parameter:-.123");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = -0.123d
                }
            });

            // Parses a positive floating point number with no digits after the decimal point and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("/parameter=123.");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = 123.0d
                }
            });

            // Parses a negative floating point number with no digits after the decimal point and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("--parameter=-123.");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new NumberParameter
                {
                    Name = "parameter",
                    Value = -123.0d
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles un-quoted strings.
        /// </summary>
        [TestMethod]
        public void StringDataTypeTest()
        {
            // Parses an un-quoted string value
            ParameterBag parameterBag = Parser.Parse("/first:abc --second=XYZ");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new StringParameter
                {
                    Name = "first",
                    Value = "abc"
                },
                new StringParameter
                {
                    Name = "second",
                    Value = "XYZ"
                }
            });
        }

        /// <summary>
        /// Tests how the parsers handles quoted strings.
        /// </summary>
        [TestMethod]
        public void QuotedStringDataTypeTest()
        {
            // Parses a quoted string value
            ParameterBag parameterBag = Parser.Parse("/parameter \"abc XYZ 123 ! § $ % & / ( ) = ? \\\"");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new StringParameter
                {
                    Name = "parameter",
                    Value = "abc XYZ 123 ! § $ % & / ( ) = ? \\"
                }
            });
        }

        /// <summary>
        /// Tests how the parser handles arrays.
        /// </summary>
        [TestMethod]
        public void ArrayDataTypeTest()
        {
            // Parses an empty array and validates that the parsed parameters are correct
            ParameterBag parameterBag = Parser.Parse("--parameter=[]");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new ArrayParameter
                {
                    Name = "parameter",
                    Value = new List<Parameter>()
                }
            });

            // Parses an array with a single element and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("/parameter [123]");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new ArrayParameter
                {
                    Name = "parameter",
                    Value = new List<Parameter>
                    {
                        new NumberParameter { Value = 123.0d }
                    }
                }
            });

            // Parses an array with all different kinds of data types and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("--parameter:[false, 123.456, abcXYZ, \"abc XYZ 123\"]");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new ArrayParameter
                {
                    Name = "parameter",
                    Value = new List<Parameter>
                    {
                        new BooleanParameter { Value = false },
                        new NumberParameter { Value = 123.456d },
                        new StringParameter { Value = "abcXYZ" },
                        new StringParameter { Value = "abc XYZ 123" }
                    }
                }
            });

            // Parses a jagged array (array of arrays) and validates that the parsed parameters are correct
            parameterBag = Parser.Parse("/parameter:[123, [abcXYZ, true]]");
            this.ValidateParseOutput(parameterBag.Parameters, new List<Parameter>
            {
                new ArrayParameter
                {
                    Name = "parameter",
                    Value = new List<Parameter>
                    {
                        new NumberParameter { Value = 123.0d },
                        new ArrayParameter
                        {
                            Value = new List<Parameter>
                            {
                                new StringParameter { Value = "abcXYZ" },
                                new BooleanParameter { Value = true }
                            }
                        }
                    }
                }
            });
        }

        #endregion

        #region Command Line Parameter Injection Test Methods

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into an empty class.
        /// </summary>
        [TestMethod]
        public void EmptyParameterContainerInjectionTest()
        {
            // Parses an empty command line and validates that the object was properly created
            EmptyParameterContainer emptyParameterContainer = Parser.Parse<EmptyParameterContainer>(string.Empty);
            Assert.IsNotNull(emptyParameterContainer);

            // Parses several command line parameters and validates that the object was properly created
            emptyParameterContainer = Parser.Parse<EmptyParameterContainer>("/on /key:value --auto --parameter=123 -aFl");
            Assert.IsNotNull(emptyParameterContainer);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class that has a single non-default constructor.
        /// </summary>
        [TestMethod]
        public void SingleConstructorParameterContainerInjectionTest()
        {
            // Parses command line parameters and validates that the object was properly created
            SingleConstructorParameterContainer singleConstructorParameterContainer = Parser.Parse<SingleConstructorParameterContainer>("/first \"abc XYZ\" --second:-123.456");
            Assert.IsNotNull(singleConstructorParameterContainer);
            Assert.AreEqual(singleConstructorParameterContainer.First, "abc XYZ");
            Assert.AreEqual(singleConstructorParameterContainer.Second, -123);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class that has a multiple non-default constructor.
        /// </summary>
        [TestMethod]
        public void MultipleConstructorParameterContainerInjectionTest()
        {
            // Parses a single command line parameter and validates that the correct constructor was called
            MultipleConstructorsParameterContainer multipleConstructorsParameterContainer = Parser.Parse<MultipleConstructorsParameterContainer>("/first true");
            Assert.AreEqual(multipleConstructorsParameterContainer.ConstructorCalled, 1);

            // Parses two command line parameters and validates that the correct constructor was called
            multipleConstructorsParameterContainer = Parser.Parse<MultipleConstructorsParameterContainer>("/first true --second=abc");
            Assert.AreEqual(multipleConstructorsParameterContainer.ConstructorCalled, 2);

            // Parses three command line parameters and validates that the correct constructor was called
            multipleConstructorsParameterContainer = Parser.Parse<MultipleConstructorsParameterContainer>("/first true --second=abc /third:123");
            Assert.AreEqual(multipleConstructorsParameterContainer.ConstructorCalled, 3);
        }

        [TestMethod]
        public void SimplePropertyParameterContainerInjectionTest()
        {
            // Parses command line parameters and validates that the object was properly created
            SimplePropertyParameterContainer simplePropertyParameterContainer = Parser.Parse<SimplePropertyParameterContainer>("/string \"abc XYZ\" --number:123.456 --boolean=true");
            Assert.IsNotNull(simplePropertyParameterContainer);
            Assert.AreEqual(simplePropertyParameterContainer.String, "abc XYZ");
            Assert.AreEqual(simplePropertyParameterContainer.Number, 123.456d);
            Assert.AreEqual(simplePropertyParameterContainer.Boolean, true);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Represents an empty parameter container, which is used to test injection into an empty class.
        /// </summary>
        private class EmptyParameterContainer { }
        
        /// <summary>
        /// Represents a parameter container, which is used to test single constructors.
        /// </summary>
        private class SingleConstructorParameterContainer
        {
            #region Constructors

            /// <summary>
            /// Initializes a new <see cref="SingleConstructorParameterContainer"/> instance.
            /// </summary>
            /// <param name="first">The first command line parameter.</param>
            /// <param name="second">The second command line parameter.</param>
            public SingleConstructorParameterContainer(string first, int second)
            {
                this.First = first;
                this.Second = second;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the "first" command line parameter.
            /// </summary>
            public string First { get; set; }

            /// <summary>
            /// Gets or sets the "second" command line parameter.
            /// </summary>
            public int Second { get; set; }

            #endregion
        }

        /// <summary>
        /// Represents a parameter container, which is used to test multiple constructors.
        /// </summary>
        private class MultipleConstructorsParameterContainer
        {
            #region Constructors

            /// <summary>
            /// Initializes a new <see cref="MultipleConstructorsParameterContainer"/> instance.
            /// </summary>
            /// <param name="first">The first command line parameter.</param>
            public MultipleConstructorsParameterContainer(bool first)
            {
                this.ConstructorCalled = 1;
            }

            /// <summary>
            /// Initializes a new <see cref="MultipleConstructorsParameterContainer"/> instance.
            /// </summary>
            /// <param name="first">The first command line parameter.</param>
            /// <param name="second">The second command line parameter.</param>
            public MultipleConstructorsParameterContainer(bool first, string second)
            {
                this.ConstructorCalled = 2;
            }

            /// <summary>
            /// Initializes a new <see cref="MultipleConstructorsParameterContainer"/> instance.
            /// </summary>
            /// <param name="first">The first command line parameter.</param>
            /// <param name="second">The second command line parameter.</param>
            /// <param name="third">The third command line parameter.</param>
            public MultipleConstructorsParameterContainer(bool first, string second, int third)
            {
                this.ConstructorCalled = 3;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the number of the constructor that has been called, which is used to validate that the correct constructor has been called.
            /// </summary>
            public int ConstructorCalled { get; set; }

            #endregion
        }

        /// <summary>
        /// Represents a parameter container, which is used to test the injection of simple data types.
        /// </summary>
        private class SimplePropertyParameterContainer
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the "number" command line parameter.
            /// </summary>
            [ParameterName("number")]
            public double Number { get; set; }

            /// <summary>
            /// Gets or sets the "boolean" command line parameter.
            /// </summary>
            [ParameterName("boolean")]
            public bool Boolean { get; set; }

            /// <summary>
            /// Gets or sets the "string" command line parameter.
            /// </summary>
            [ParameterName("string")]
            public string String { get; set; }

            #endregion
        }

        #endregion
    }
}