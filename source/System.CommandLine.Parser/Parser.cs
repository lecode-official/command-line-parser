
#region Using Directives

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents a parser for command line arguments.
    /// </summary>
    public class Parser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="description">The description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.</param>
        public Parser(string description)
        {
            this.description = description;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Contains the positional arguments of the parser.
        /// </summary>
        private readonly List<PositionalArgument> positionalArguments = new List<PositionalArgument>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        public void AddPositionalArgument<T>(string name) => this.AddPositionalArgument<T>(name, name);

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        public void AddPositionalArgument<T>(string name, string help) => this.AddPositionalArgument<T>(name, name, help);

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="destination">The name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination is <c>null<c>, empty, only consists of white spaces, then a <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a positional argument with the same name or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        public void AddPositionalArgument<T>(string name, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already a positional argument with the same name or destination
            if (this.positionalArguments.Any(positionalArgument => positionalArgument.Name == name))
                throw new InvalidOperationException("There is already a positional argument with the same name.");
            if (this.positionalArguments.Any(positionalArgument => positionalArgument.Destination == destination))
                throw new InvalidOperationException("There is already a positional argument with the same name.");

            // Adds the positional argument to the parser
            this.positionalArguments.Add(new PositionalArgument(name, destination, help, typeof(T)));
        }

        #endregion
    }
}