
#region Using Directives

using System;
using System.Collections.Generic;
using System.CommandLine.Parser;
using System.CommandLine.Parser.Parameters;
using System.Linq;
using System.Runtime.InteropServices;
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
        private static void WriteParameters(IDictionary<string, Parameter> parameters, int indentLevel = 0)
        {
            // Cycles over all the parameters and prints them out
            foreach (string parameterName in parameters.Keys)
            {
                // Gets the actual parameter
                Parameter parameter = parameters[parameterName];

                // Indents the line according to the current indent level
                System.Console.Write(string.Join(string.Empty, Enumerable.Range(0, 4 * indentLevel).Select(index => ' ')));

                // Checks if the parameter is a simple data type, if so it is printed out
                BooleanParameter booleanParameter = parameter as BooleanParameter;
                if (booleanParameter != null)
                    System.Console.WriteLine($"{parameterName} (Boolean): {booleanParameter.Value}");
                NumberParameter numberParameter = parameter as NumberParameter;
                if (numberParameter != null)
                    System.Console.WriteLine($"{parameterName} (Number): {numberParameter.Value}");
                StringParameter stringParameter = parameter as StringParameter;
                if (stringParameter != null)
                    System.Console.WriteLine($"{parameterName} (String): {stringParameter.Value}");
                DefaultParameter defaultParameter = parameter as DefaultParameter;
                if (defaultParameter != null)
                    System.Console.WriteLine($"Default parameter: {defaultParameter.Value}");

                // Checks if the parameter is of type array, if so then its contents are printed out recursively
                ArrayParameter arrayParameter = parameter as ArrayParameter;
                if (arrayParameter != null)
                {
                    System.Console.Write($"{parameterName} (Array):");
                    Program.WriteParameters(Enumerable.Range(0, arrayParameter.Value.Count()).ToDictionary(index => $"[{index}]", index => arrayParameter.Value.ElementAt(index)), indentLevel + 1);
                }
            }
        }

        /// <summary>
        /// The asynchronous entry point to the command line parser console application, which allows the execution of asynchronous code.
        /// </summary>
        private static async Task MainAsync()
        {
            // Prints out the command line parameters passed to the application
            System.CommandLine.Parser.CommandLineParser parser = new System.CommandLine.Parser.CommandLineParser();
            System.Console.WriteLine("Command line parameters:");
            System.Console.WriteLine(Program.GetCommandLineArguments());
            System.Console.WriteLine();

            // Parses the command line parameters passed to the application and prints them out
            ParameterBag parameterBag = await parser.ParseAsync(Program.GetCommandLineArguments());
            if (parameterBag.Parameters.Any())
            {
                System.Console.WriteLine("Parameter bag content:");
                Program.WriteParameters(parameterBag.Parameters);
                System.Console.WriteLine();
            }

            // Parses the command line parameters passed to the application, injects them into a strongly typed object, and prints them out
            CommandLineParameters commandLineParameters = await parser.BindAsync<CommandLineParameters>(Program.GetCommandLineArguments());
            System.Console.WriteLine("Parameters:");
            System.Console.WriteLine($"Method: {commandLineParameters.Method}");
            System.Console.WriteLine($"Number of bytes: {commandLineParameters.NumberOfBytes}");
            System.Console.WriteLine($"Output file name: {commandLineParameters.OutputFileName}");
            System.Console.WriteLine($"Force: {commandLineParameters.Force}");
            System.Console.WriteLine();

            // Waits for the user to press a key before the program is exited
            System.Console.Write("Press any key to exit...");
            System.Console.ReadKey();
        }

        /// <summary>
        /// Gets the arguments that were passed to the program on the command line.
        /// </summary>
        /// <returns>
        /// Returns a pointer to a buffer allocated and owned by the operating system, which contains the command line arguments passed to the program. Since this
        /// buffer was allocated by the operating system, it must not be freed.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetCommandLine();

        /// <summary>
        /// Gets the command line arguments passed to the program.
        /// </summary>
        /// <returns>Returns the command line arguments passed to the program on the command line.</returns>
        private static string GetCommandLineArguments()
        {
            // Gets a pointer to a buffer, that contains the command line arguments
            IntPtr commandLineArgumentsBuffer = Program.GetCommandLine();

            // Marshals the contents of the buffer to a CLR string and returns it
            return Marshal.PtrToStringUni(commandLineArgumentsBuffer);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// The entry point to the command line parser console sample application.
        /// </summary>
        /// <param name="args">The command line parameters, which are passed to the application.</param>
        public static void Main(string[] args) => Program.MainAsync().Wait();

        #endregion

        #region Nested Types

        /// <summary>
        /// Represents a class that is used to inject the command line parameters into.
        /// </summary>
        private class CommandLineParameters
        {
            #region Public Properties

            /// <summary>
            /// Gets the "method" command line parameter.
            /// </summary>
            [ParameterName("method")]
            public string Method { get; set; }

            /// <summary>
            /// Gets or sets the "numberOfBytes" command line parameter.
            /// </summary>
            [ParameterName("numberOfBytes")]
            public int NumberOfBytes { get; set; }

            /// <summary>
            /// Gets or sets the "out" command line parameter.
            /// </summary>
            [ParameterName("out")]
            public string OutputFileName { get; set; }

            /// <summary>
            /// Gets or sets the "f" command line parameter.
            /// </summary>
            [ParameterName("f")]
            public bool Force { get; set; }

            #endregion
        }

        #endregion
    }
}