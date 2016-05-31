﻿
#region Using Directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.CommandLine.Parser.Parameters;
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
        private void ValidateParseOutput(IDictionary<string, Parameter> parameters, IDictionary<string, Parameter> expectedParameters)
        {
            // Validates that the amount of parameters in the parameter bag and the expected parameter bag are the same
            Assert.AreEqual(expectedParameters.Count(), parameters.Count());
            if (parameters.Count() != expectedParameters.Count())
                return;

            // Validates that both parameter sets have the same parameter names
            Assert.IsTrue(parameters.Keys.All(key => expectedParameters.ContainsKey(key)));
            Assert.IsTrue(expectedParameters.Keys.All(key => parameters.ContainsKey(key)));

            // Cycles over all parameters in the parameter bag and validates them against the expected parameters
            foreach (string parameterName in parameters.Keys)
            {
                // Gets the two parameter at the current position
                Parameter parameter = parameters[parameterName];
                Parameter expectedParameter = expectedParameters[parameterName];

                // Validates that the parameter has the same type as the expected parameter
                Assert.IsInstanceOfType(parameter, expectedParameter.GetType());
                if (parameter.GetType() != expectedParameter.GetType())
                    continue;
                
                // Checks if the parameter is a simple data type, if so its value is validated againts the expected parameter
                BooleanParameter booleanParameter = parameter as BooleanParameter;
                if (booleanParameter != null)
                    Assert.AreEqual((expectedParameter as BooleanParameter).Value, booleanParameter.Value);
                NumberParameter numberParameter = parameter as NumberParameter;
                if (numberParameter != null)
                    Assert.AreEqual((expectedParameter as NumberParameter).Value, numberParameter.Value);
                StringParameter stringParameter = parameter as StringParameter;
                if (stringParameter != null)
                    Assert.AreEqual((expectedParameter as StringParameter).Value, stringParameter.Value);

                // Checks if the parameter is of type array, if so then its contents are validated recursively
                ArrayParameter arrayParameter = parameter as ArrayParameter;
                if (arrayParameter != null)
                {
                    ArrayParameter expectedArrayParameter = expectedParameter as ArrayParameter;
                    this.ValidateParseOutput(
                        Enumerable.Range(0, arrayParameter.Value.Count()).ToDictionary(index => $"[{index}]", index => arrayParameter.Value.ElementAt(index)),
                        Enumerable.Range(0, expectedArrayParameter.Value.Count()).ToDictionary(index => $"[{index}]", index => expectedArrayParameter.Value.ElementAt(index)));
                }
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse(string.Empty);

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>());
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("abcXYZ");

            // Validates that the parsed parameters are correct
            Assert.AreEqual(1, parameterBag.DefaultParameters.Count());
            Assert.AreEqual("abcXYZ", parameterBag.DefaultParameters.OfType<DefaultParameter>().First().Value);
        }

        /// <summary>
        /// Tests how the parser handles multiple default parameters.
        /// </summary>
        [TestMethod]
        public void MutlipleDefaultParameterTest()
        {
            // Parses multiple default command line parameters
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("abc \"123 456\" XYZ \"789 0\"");

            // Validates that the parsed parameters are correct
            Assert.AreEqual(4, parameterBag.DefaultParameters.Count());
            Assert.AreEqual("abc", parameterBag.DefaultParameters.OfType<DefaultParameter>().ElementAt(0).Value);
            Assert.AreEqual("123 456", parameterBag.DefaultParameters.OfType<DefaultParameter>().ElementAt(1).Value);
            Assert.AreEqual("XYZ", parameterBag.DefaultParameters.OfType<DefaultParameter>().ElementAt(2).Value);
            Assert.AreEqual("789 0", parameterBag.DefaultParameters.OfType<DefaultParameter>().ElementAt(3).Value);
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/Switch");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["Switch"] = new BooleanParameter { Value = true }
            });
        }

        /// <summary>
        /// Tests how the parser handles Windows style parameters.
        /// </summary>
        [TestMethod]
        public void WindowsStyleParameterTest()
        {
            // Parses a Windows style parameter
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/Parameter:123");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["Parameter"] = new NumberParameter { Value = 123.0M }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style switches.
        /// </summary>
        [TestMethod]
        public void UnixStyleSwitchTest()
        {
            // Parses a UNIX style switch
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("--Switch");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["Switch"] = new BooleanParameter { Value = true }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style parameters.
        /// </summary>
        [TestMethod]
        public void UnixStyleParameterTest()
        {
            // Parses a UNIX style parameter
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("--Parameter=\"abc XYZ\"");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["Parameter"] = new StringParameter { Value = "abc XYZ" }
            });
        }

        /// <summary>
        /// Tests how the parser handles UNIX style flagged switches.
        /// </summary>
        [TestMethod]
        public void UnixStyleFlaggedSwitchesTest()
        {
            // Parses a UNIX style flagged switch
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("-sUtZ");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["s"] = new BooleanParameter { Value = true },
                ["U"] = new BooleanParameter { Value = true },
                ["t"] = new BooleanParameter { Value = true },
                ["Z"] = new BooleanParameter { Value = true }
            });
        }

        /// <summary>
        /// Tests how the parser handles multiple parameters.
        /// </summary>
        [TestMethod]
        public void MultipleParameterTest()
        {
            // Parses a multiple parameter
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/on /key:value --auto --parameter=123 -aFl");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["on"] = new BooleanParameter { Value = true },
                ["key"] = new StringParameter { Value = "value" },
                ["auto"] = new BooleanParameter { Value = true },
                ["parameter"] = new NumberParameter { Value = 123.0M },
                ["a"] = new BooleanParameter { Value = true },
                ["F"] = new BooleanParameter { Value = true },
                ["l"] = new BooleanParameter { Value = true }
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("\"C:\\Users\\name\\Downloads\" /key:value --auto");

            // Validates that the parsed parameters are correct
            Assert.AreEqual(1, parameterBag.DefaultParameters.Count());
            Assert.AreEqual("C:\\Users\\name\\Downloads", parameterBag.DefaultParameters.OfType<DefaultParameter>().First().Value);
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["key"] = new StringParameter { Value = "value" },
                ["auto"] = new BooleanParameter { Value = true }
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/first:false --second=true");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["first"] = new BooleanParameter { Value = false },
                ["second"] = new BooleanParameter { Value = true }
            });
        }

        /// <summary>
        /// Tests how the parser handles numbers.
        /// </summary>
        [TestMethod]
        public void NumberDataTypeTest()
        {
            // Parses a positive integer and validates that the parsed parameters are correct
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/parameter=123");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = 123.0M }
            });

            // Parses a negative integer and validates that the parsed parameters are correct
            parameterBag = parser.Parse("--parameter:-123");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = -123.0M }
            });

            // Parses a positive floating point number and validates that the parsed parameters are correct
            parameterBag = parser.Parse("/parameter 123.456");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = 123.456M }
            });

            // Parses a negative floating point number and validates that the parsed parameters are correct
            parameterBag = parser.Parse("--parameter -123.456");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = -123.456M }
            });

            // Parses a positive floating point number with no digits before the decimal point and validates that the parsed parameters are correct
            parameterBag = parser.Parse("/parameter .123");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = 0.123M }
            });

            // Parses a negative floating point number with no digits before the decimal point and validates that the parsed parameters are correct
            parameterBag = parser.Parse("--parameter:-.123");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = -0.123M }
            });

            // Parses a positive floating point number with no digits after the decimal point and validates that the parsed parameters are correct
            parameterBag = parser.Parse("/parameter=123.");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = 123.0M }
            });

            // Parses a negative floating point number with no digits after the decimal point and validates that the parsed parameters are correct
            parameterBag = parser.Parse("--parameter=-123.");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new NumberParameter { Value = -123.0M }
            });
        }

        /// <summary>
        /// Tests how the parser handles un-quoted strings.
        /// </summary>
        [TestMethod]
        public void StringDataTypeTest()
        {
            // Parses an un-quoted string value
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/first:abc --second=XYZ");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["first"] = new StringParameter { Value = "abc" },
                ["second"] = new StringParameter { Value = "XYZ" }
            });
        }

        /// <summary>
        /// Tests how the parsers handles quoted strings.
        /// </summary>
        [TestMethod]
        public void QuotedStringDataTypeTest()
        {
            // Parses a quoted string value
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("/parameter \"abc XYZ 123 ! § $ % & / ( ) = ? \\\"");

            // Validates that the parsed parameters are correct
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new StringParameter { Value = "abc XYZ 123 ! § $ % & / ( ) = ? \\" }
            });
        }

        /// <summary>
        /// Tests how the parser handles arrays.
        /// </summary>
        [TestMethod]
        public void ArrayDataTypeTest()
        {
            // Parses an empty array and validates that the parsed parameters are correct
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ParameterBag parameterBag = parser.Parse("--parameter=[]");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new ArrayParameter { Value = new List<Parameter>() }
            });

            // Parses an array with a single element and validates that the parsed parameters are correct
            parameterBag = parser.Parse("/parameter [123]");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new ArrayParameter
                {
                    Value = new List<Parameter> { new NumberParameter { Value = 123.0M } }
                }
            });

            // Parses an array with all different kinds of data types and validates that the parsed parameters are correct
            parameterBag = parser.Parse("--parameter:[false, 123.456, abcXYZ, \"abc XYZ 123\"]");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new ArrayParameter
                {
                    Value = new List<Parameter>
                    {
                        new BooleanParameter { Value = false },
                        new NumberParameter { Value = 123.456M },
                        new StringParameter { Value = "abcXYZ" },
                        new StringParameter { Value = "abc XYZ 123" }
                    }
                }
            });

            // Parses a jagged array (array of arrays) and validates that the parsed parameters are correct
            parameterBag = parser.Parse("/parameter:[123, [abcXYZ, true]]");
            this.ValidateParseOutput(parameterBag.Parameters, new Dictionary<string, Parameter>
            {
                ["parameter"] = new ArrayParameter
                {
                    Value = new List<Parameter>
                    {
                        new NumberParameter { Value = 123.0M },
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
            CommandLineParameterParser parser = new CommandLineParameterParser();
            EmptyParameterContainer emptyParameterContainer = parser.Parse<EmptyParameterContainer>(string.Empty);
            Assert.IsNotNull(emptyParameterContainer);

            // Parses several command line parameters and validates that the object was properly created
            emptyParameterContainer = parser.Parse<EmptyParameterContainer>("/on /key:value --auto --parameter=123 -aFl");
            Assert.IsNotNull(emptyParameterContainer);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class that has a single non-default constructor.
        /// </summary>
        [TestMethod]
        public void SingleConstructorParameterContainerInjectionTest()
        {
            // Parses command line parameters and validates that the object was properly created
            CommandLineParameterParser parser = new CommandLineParameterParser();
            SingleConstructorParameterContainer singleConstructorParameterContainer = parser.Parse<SingleConstructorParameterContainer>("/first \"abc XYZ\" --second:-123.456");
            Assert.IsNotNull(singleConstructorParameterContainer);
            Assert.AreEqual("abc XYZ", singleConstructorParameterContainer.First);
            Assert.AreEqual(-123, singleConstructorParameterContainer.Second);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class that has a multiple non-default constructor.
        /// </summary>
        [TestMethod]
        public void MultipleConstructorParameterContainerInjectionTest()
        {
            // Parses a single command line parameter and validates that the correct constructor was called
            CommandLineParameterParser parser = new CommandLineParameterParser();
            MultipleConstructorsParameterContainer multipleConstructorsParameterContainer = parser.Parse<MultipleConstructorsParameterContainer>("/first true");
            Assert.AreEqual(1, multipleConstructorsParameterContainer.ConstructorCalled);

            // Parses two command line parameters and validates that the correct constructor was called
            multipleConstructorsParameterContainer = parser.Parse<MultipleConstructorsParameterContainer>("/first true --second=abc");
            Assert.AreEqual(2, multipleConstructorsParameterContainer.ConstructorCalled);

            // Parses three command line parameters and validates that the correct constructor was called
            multipleConstructorsParameterContainer = parser.Parse<MultipleConstructorsParameterContainer>("/first true --second=abc /third:123");
            Assert.AreEqual(3, multipleConstructorsParameterContainer.ConstructorCalled);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class with some simple-type properties.
        /// </summary>
        [TestMethod]
        public void SimplePropertyParameterContainerInjectionTest()
        {
            // Parses command line parameters and validates that the object was properly created
            CommandLineParameterParser parser = new CommandLineParameterParser();
            SimplePropertyParameterContainer simplePropertyParameterContainer = parser.Parse<SimplePropertyParameterContainer>("/string \"abc XYZ\" --number:123.456 --boolean=true /enum:Monday");
            Assert.IsNotNull(simplePropertyParameterContainer);
            Assert.AreEqual("abc XYZ", simplePropertyParameterContainer.String);
            Assert.AreEqual(123.456d, simplePropertyParameterContainer.Number);
            Assert.AreEqual(true, simplePropertyParameterContainer.Boolean);
            Assert.AreEqual(DayOfWeek.Monday, simplePropertyParameterContainer.Enumeration);
        }

        /// <summary>
        /// Tests how the parser handles the command line parameter injection into a class with array-type properties.
        /// </summary>
        [TestMethod]
        public void ArrayPropertyParameterContainerInjectionTest()
        {
            // Parses command line parameters that contain an array of boolean values and validates that the object was properly created
            CommandLineParameterParser parser = new CommandLineParameterParser();
            ArrayPropertyParameterContainer arrayPropertyParameterContainer = parser.Parse<ArrayPropertyParameterContainer>("/BooleanCollection [true, 1, false, 0]");
            Assert.IsNotNull(arrayPropertyParameterContainer.BooleanCollection);
            Assert.IsTrue(arrayPropertyParameterContainer.BooleanCollection.ElementAt(0));
            Assert.IsTrue(arrayPropertyParameterContainer.BooleanCollection.ElementAt(1));
            Assert.IsFalse(arrayPropertyParameterContainer.BooleanCollection.ElementAt(2));
            Assert.IsFalse(arrayPropertyParameterContainer.BooleanCollection.ElementAt(3));

            // Parses command line parameters that contain an array of string values and validates that the object was properly created
            arrayPropertyParameterContainer = parser.Parse<ArrayPropertyParameterContainer>("/StringCollection [abc, 123, \"abc XYZ\", 123.456, true]");
            Assert.IsNotNull(arrayPropertyParameterContainer.StringCollection);
            Assert.AreEqual("abc", arrayPropertyParameterContainer.StringCollection[0]);
            Assert.AreEqual("123", arrayPropertyParameterContainer.StringCollection[1]);
            Assert.AreEqual("abc XYZ", arrayPropertyParameterContainer.StringCollection[2]);
            Assert.AreEqual("123.456", arrayPropertyParameterContainer.StringCollection[3]);
            Assert.AreEqual("True", arrayPropertyParameterContainer.StringCollection[4]);

            // Parses command line parameters that contain an array of number values and validates that the object was properly created
            arrayPropertyParameterContainer = parser.Parse<ArrayPropertyParameterContainer>("--NumberCollection:[123.456, 123, true, \"456\"]");
            Assert.IsNotNull(arrayPropertyParameterContainer.NumberCollection);
            Assert.AreEqual(123.456d, arrayPropertyParameterContainer.NumberCollection.ElementAt(0));
            Assert.AreEqual(123.0d, arrayPropertyParameterContainer.NumberCollection.ElementAt(1));
            Assert.AreEqual(1.0d, arrayPropertyParameterContainer.NumberCollection.ElementAt(2));
            Assert.AreEqual(456.0d, arrayPropertyParameterContainer.NumberCollection.ElementAt(3));

            // Parses command line parameters that contain an array of enumeration values and validates that the object was properly created
            arrayPropertyParameterContainer = parser.Parse<ArrayPropertyParameterContainer>("/EnumerationCollection [Monday, \"Tuesday\", Wednesday, \"Thursday\", Friday, \"Saturday\", Sunday]");
            Assert.IsNotNull(arrayPropertyParameterContainer.EnumerationCollection);
            Assert.AreEqual(DayOfWeek.Monday, arrayPropertyParameterContainer.EnumerationCollection[0]);
            Assert.AreEqual(DayOfWeek.Tuesday, arrayPropertyParameterContainer.EnumerationCollection[1]);
            Assert.AreEqual(DayOfWeek.Wednesday, arrayPropertyParameterContainer.EnumerationCollection[2]);
            Assert.AreEqual(DayOfWeek.Thursday, arrayPropertyParameterContainer.EnumerationCollection[3]);
            Assert.AreEqual(DayOfWeek.Friday, arrayPropertyParameterContainer.EnumerationCollection[4]);
            Assert.AreEqual(DayOfWeek.Saturday, arrayPropertyParameterContainer.EnumerationCollection[5]);
            Assert.AreEqual(DayOfWeek.Sunday, arrayPropertyParameterContainer.EnumerationCollection[6]);
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

            /// <summary>
            /// Gets or sets the "enum" command line parameter.
            /// </summary>
            [ParameterName("enum")]
            public DayOfWeek Enumeration { get; set; }

            #endregion
        }

        /// <summary>
        /// Represents a parameter container, which is used to test the injection of array data types.
        /// </summary>
        private class ArrayPropertyParameterContainer
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the "BooleanCollection" command line parameter.
            /// </summary>
            public IEnumerable<bool> BooleanCollection { get; set; }

            /// <summary>
            /// Gets or sets the "StringCollection" command line parameter.
            /// </summary>
            public List<string> StringCollection { get; set; }

            /// <summary>
            /// Gets or sets the "NumberCollection" command line parameter.
            /// </summary>
            public HashSet<double> NumberCollection { get; set; }

            /// <summary>
            /// Gets or sets the "EnumerationCollection" command line parameter.
            /// </summary>
            public IList<DayOfWeek> EnumerationCollection { get; set; }

            #endregion
        }

        #endregion
    }
}