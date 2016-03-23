
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.CommandLine.Parser.Antlr;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a parser, which is able to parse command line parameters and convert them to strongly-typed .NET data types.
    /// </summary>
    public static class Parser
    {
        #region Public Static Methods
        
        /// <summary>
        /// Parses the specified command line parameters.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public static ParameterBag Parse(string commandLineParameters)
        {
            // Parses the command line parameters using the ANTRL4 generated parsers
            CommandLineLexer lexer = new CommandLineLexer(new AntlrInputStream(new StringReader(commandLineParameters)));
            CommandLineParser parser = new CommandLineParser(new CommonTokenStream(lexer)) { BuildParseTree = true };
            IParseTree parseTree = parser.commandLine();
            CommandLineVisitor commandLineVisitor = new CommandLineVisitor();
            IEnumerable<Parameter> parameters = commandLineVisitor.Visit(parseTree);

            // Returns the parsed parameters wrapped in a parameter bag
            return new ParameterBag
            {
                CommandLineParameters = commandLineParameters,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Parses the command line parameters that have been passed to the program.
        /// </summary>
        /// <returns>Returns the parsed parameters.</returns>
        public static ParameterBag Parse() => Parser.Parse(Environment.CommandLine);

        /// <summary>
        /// Parses the specified command line parameters asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public static Task<ParameterBag> ParseAsync(string commandLineParameters) => Task.Run(() => Parser.Parse(commandLineParameters));

        /// <summary>
        /// Parses the command line parameters that have been passed to the program asynchronously.
        /// </summary>
        /// <returns>Returns the parsed parameters.</returns>
        public static Task<ParameterBag> ParseAsync() => Task.Run(() => Parser.Parse());

        #endregion
    }
}