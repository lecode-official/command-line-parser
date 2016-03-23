
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
        
        #endregion

        #region Default Parameter Test Methods
        
        #endregion

        #region Parameter Test Methods

        #endregion

        #region Data Type Test Methods

        #endregion
    }
}