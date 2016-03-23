
#region Using Directives

using System;
using System.Collections.Generic;
using System.CommandLine.Parser;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace CommandLineParser.Samples.Console
{
    /// <summary>
    /// Represents the command line parser console sample application.
    /// </summary>
    public static class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Prints out the specified parameters on the console.
        /// </summary>
        /// <param name="parameters">The parameters that are to be printed out.</param>
        private static void WriteParameters(IEnumerable<Parameter> parameters, int indentLevel = 0)
        {
            // Cycles over all the parameters and prints them out
            foreach (Parameter parameter in parameters)
            {
                // Indents the line according to the current indent level
                System.Console.Write(string.Join(string.Empty, Enumerable.Range(0, 4 * indentLevel).Select(index => ' ')));

                // Checks if the parameter is a simple data type, if so it is printed out
                BooleanParameter booleanParameter = parameter as BooleanParameter;
                if (booleanParameter != null)
                    System.Console.WriteLine($"{booleanParameter.Name} (Boolean): {booleanParameter.Value}");
                NumberParameter numberParameter = parameter as NumberParameter;
                if (numberParameter != null)
                    System.Console.WriteLine($"{numberParameter.Name} (Number): {numberParameter.Value}");
                StringParameter stringParameter = parameter as StringParameter;
                if (stringParameter != null)
                    System.Console.WriteLine($"{stringParameter.Name} (String): {stringParameter.Value}");
                DefaultParameter defaultParameter = parameter as DefaultParameter;
                if (defaultParameter != null)
                    System.Console.WriteLine($"Default parameter: {defaultParameter.Value}");

                // Checks if the parameter is of type array, if so then its contents are printed out recursively
                ArrayParameter arrayParameter = parameter as ArrayParameter;
                if (arrayParameter != null)
                {
                    System.Console.Write($"{arrayParameter.Name} (Array):");
                    Program.WriteParameters(arrayParameter.Value, indentLevel + 1);
                }
            }
        }

        /// <summary>
        /// The asynchronous entry point to the command line parser console application, which allows the execution of asynchronous code.
        /// </summary>
        private static async Task MainAsync()
        {
            // Prints out the command line parameters passed to the application
            System.Console.WriteLine("Command line parameters:");
            System.Console.WriteLine(Environment.CommandLine);
            System.Console.WriteLine();

            // Parses the command line arguments passed to the application and prints them out
            ParameterBag parameterBag = await Parser.ParseAsync();
            if (parameterBag.Parameters.Any())
            {
                System.Console.WriteLine("Parameters:");
                Program.WriteParameters(parameterBag.Parameters);
                System.Console.WriteLine();
            }

            // Waits for the user to press a key before the program is exited
            System.Console.Write("Press any key to exit...");
            System.Console.ReadKey();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// The entry point to the command line parser console sample application.
        /// </summary>
        /// <param name="args">The command line parameters, which are passed to the application.</param>
        public static void Main(string[] args) => Program.MainAsync().Wait();
        
        #endregion
    }
}