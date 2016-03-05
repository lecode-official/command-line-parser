
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CommandLine.Parser.Antlr;
using System.IO;

#endregion

namespace System.CommandLine.Parser.UnitTests
{
    /// <summary>
    /// Represents a unit testing class, which contains all of the unit tests for the command line parser.
    /// </summary>
    [TestClass]
    public class AntlrParserUnitTests
    {
        #region Test Methods

        /// <summary>
        /// Test if the ANTLR4 lexer and parser can handle empty command line parameters.
        /// </summary>
        [TestMethod]
        public void EmptyCommandLineParamentersTest()
        {
            AntlrInputStream stream = new AntlrInputStream(new StringReader(string.Empty));
            ITokenSource lexer = new CommandLineLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CommandLineParser parser = new CommandLineParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.commandLine();

        }

        #endregion
    }
}