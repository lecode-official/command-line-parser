
namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a parser, which is able to parse command line parameters and convert them to strongly-typed .NET data types.
    /// </summary>
    public static class CommandLineParser
    {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the command line parameters that have been passed to the program via the command line.
        /// </summary>
        /// <returns>Returns the command line parameters that have been passed to the program via the command line.</returns>
        public static string RetrieveCommandLineParameters()
        {
            // Gets the command line parameters that have been passed to the program
            string commandLineParameters = Environment.CommandLine;

            // The command line parameters start with image file name, from which the program was loaded, this is unuseful during parsing and is therefore removed


            // Returns the retrieved command line parameters
            return commandLineParameters;
        }

        #endregion
    }
}