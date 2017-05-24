
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
        private readonly List<IArgument> positionalArguments = new List<IArgument>();

        /// <summary>
        /// Contains the arguments of the parser.
        /// </summary>
        private readonly List<IArgument> arguments = new List<IArgument>();

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
            if (this.positionalArguments.Any(positionalArgument => string.Compare(positionalArgument.Name, name, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a positional argument with the same name.");
            if (this.positionalArguments.Any(positionalArgument => positionalArgument.Destination == destination))
                throw new InvalidOperationException("There is already a positional argument with the same destination.");

            // Adds the positional argument to the parser
            this.positionalArguments.Add(new PositionalArgument<T>(name, destination, help));
        }

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name) => this.AddArgument<T>(name, name);

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name, string alias) => this.AddArgument<T>(name, alias, string.Empty);

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the alias are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name, string alias, string help) => this.AddArgument<T>(name, alias, name, help);

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, or the destination are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name, string alias, string destination, string help) => this.AddArgument<T>(name, alias, destination, help, default(T));

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, the destination, or the default value are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name, string alias, string destination, string help, T defaultValue) => this.AddArgument<T>(name, alias, destination, help, defaultValue, (oldValue, newValue) => newValue);

        /// <summary>
        /// Adds an argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <param name="duplicateResolutionPolicy">A callback function, which is invoked when the same argument was sepcified twice.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, the destination, the default value, or the duplicate resolution policy are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, the same alias, or the same destination, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public void AddArgument<T>(string name, string alias, string destination, string help, T defaultValue, Func<T, T, T> duplicateResolutionPolicy)
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
            if (duplicateResolutionPolicy == null)
                throw new ArgumentNullException(nameof(duplicateResolutionPolicy));

            // Checks if there is already an argument with the same name, the same alias, or destination
            if (this.arguments.Any(argument => string.Compare(argument.Name, name, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a argument with the same name.");
            if (this.arguments.Any(argument => string.Compare(argument.Alias, alias, this.Options.IgnoreCase) == 0))
                throw new InvalidOperationException("There is already a argument with the same alias.");
            if (this.arguments.Any(argument => argument.Destination == destination))
                throw new InvalidOperationException("There is already a argument with the same destination.");

            // Adds the argument to the parser
            this.arguments.Add(new Argument<T>(name, alias, destination, help, defaultValue, duplicateResolutionPolicy));
        }

        #endregion
    }
}