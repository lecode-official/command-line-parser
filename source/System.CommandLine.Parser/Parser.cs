
// TODO: All arguments have to be checked whether they have the same names, destinations, or aliases (not just within their type)

#region Using Directives

using System.Collections.Generic;
using System.CommandLine.Arguments;
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
        public Parser()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="description">The description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.</param>
        public Parser(string description)
            : this(description, ParserOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="description">The description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.</param>
        /// <param name="options">The options of the command line parser.</param>
        /// <exception cref="ArgumentNullException">If the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public Parser(string description, ParserOptions options)
        {
            // Validates the arguments
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // Stores the arguments for later use
            this.description = description;
            this.Options = options;
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
        private readonly List<Argument> positionalArguments = new List<Argument>();

        /// <summary>
        /// Contains the named arguments of the parser.
        /// </summary>
        private readonly List<Argument> namedArguments = new List<Argument>();

        /// <summary>
        /// Contains the flag arguments of the parser.
        /// </summary>
        private readonly List<Argument> flagArguments = new List<Argument>();

        /// <summary>
        /// Contains the sub-commands of the parser (which in turn consist of a parser, which is able to parse the arguments of the command).
        /// </summary>
        private readonly Dictionary<string, Parser> commands = new Dictionary<string, Parser>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the options of the command line parser.
        /// </summary>
        public ParserOptions Options { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddPositionalArgument<T>(string name) => this.AddPositionalArgument<T>(name, name);

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddPositionalArgument<T>(string name, string help) => this.AddPositionalArgument<T>(name, name, help);

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="destination">
        /// The name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.
        /// </param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination is <c>null<c>, empty, only consists of white spaces, then a <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a positional argument with the same name or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddPositionalArgument<T>(string name, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already a positional argument with the same name or destination
            if (this.positionalArguments.Any(positionalArgument => string.Compare(positionalArgument.Name, name, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a positional argument with the same name.");
            if (this.positionalArguments.Any(positionalArgument => positionalArgument.Destination == destination))
                throw new InvalidOperationException("There is already a positional argument with the same destination.");

            // Adds the positional argument to the parser
            this.positionalArguments.Add(new PositionalArgument<T>(name, destination, help));

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name) => this.AddNamedArgument<T>(name, name);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias) => this.AddNamedArgument<T>(name, alias, string.Empty);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string help) => this.AddNamedArgument<T>(name, alias, name, help);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, or the destination are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string destination, string help) => this.AddNamedArgument<T>(name, alias, destination, help, default(T));

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, the destination, or the default value are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string destination, string help, T defaultValue) => this.AddNamedArgument<T>(name, alias, destination, help, defaultValue, null);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <param name="duplicateResolutionPolicy">A callback function, which is invoked when the same argument was specified more than once.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, the destination, or the default value are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a named argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string destination, string help, T defaultValue, Func<T, T, T> duplicateResolutionPolicy)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));
            if (defaultValue == null)
                throw new ArgumentNullException(nameof(defaultValue));

            // Checks if there is already a named argument with the same name, the same alias, or destination
            if (this.namedArguments.Any(argument => string.Compare(argument.Name, name, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a named argument with the same name.");
            if (this.namedArguments.Any(argument => string.Compare(argument.Alias, alias, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a named argument with the same alias.");
            if (this.namedArguments.Any(argument => argument.Destination == destination))
                throw new InvalidOperationException("There is already a named argument with the same destination.");

            // Adds the argument to the parser
            this.namedArguments.Add(new NamedArgument<T>(name, alias, destination, help, defaultValue, duplicateResolutionPolicy));

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, or the destination are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a flag argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already a named argument with the same name, the same alias, or destination
            if (this.flagArguments.Any(argument => string.Compare(argument.Name, name, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a flag argument with the same name.");
            if (this.flagArguments.Any(argument => string.Compare(argument.Alias, alias, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a flag argument with the same alias.");
            if (this.flagArguments.Any(argument => argument.Destination == destination))
                throw new InvalidOperationException("There is already a flag argument with the same destination.");

            // Adds the argument to the parser
            this.flagArguments.Add(new FlagArgument<T>(name, alias, destination, help));

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a flag argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name) => this.AddFlagArgument<T>(name, name);

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a flag argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias) => this.AddFlagArgument<T>(name, alias, string.Empty);

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a flag argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias, string help) => this.AddFlagArgument<T>(name, alias, name, help);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name) => this.AddCommand(name, null, this.Options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description for the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string description) => this.AddCommand(name, description, this.Options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="options">The parser options for the command.</param>
        /// <exception cref="ArgumentNullException">If the name or the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, ParserOptions options) => this.AddCommand(name, null, options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description for the command.</param>
        /// <param name="options">The parser options for the command.</param>
        /// <exception cref="ArgumentNullException">If the name or the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string description, ParserOptions options)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // Checks if there is already a command with the same name
            if (this.commands.ContainsKey(name))
                throw new InvalidOperationException("There is already a command with the same name.");

            // Creates the parser for the new command and adds it to the list of commands
            Parser commandArgumentsParser = new Parser(description, options);
            this.commands.Add(name, commandArgumentsParser);

            // Returns the created parser
            return commandArgumentsParser;
        }

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="parser">The parser that is used to parse the arguments of the command.</param>
        /// <exception cref="ArgumentNullException">If the name or the parser are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, Parser parser)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            // Checks if there is already a command with the same name
            if (this.commands.ContainsKey(name))
                throw new InvalidOperationException("There is already a command with the same name.");

            // Adds the new command to the list of commands
            this.commands.Add(name, parser);

            // Returns the parser
            return parser;
        }

        #endregion
    }
}