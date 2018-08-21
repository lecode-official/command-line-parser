
#region Using Directives

using System.Collections.Generic;
using System.CommandLine.Arguments;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents the result of parsing. It contains all parses argument values and all parsed sub commands.
    /// </summary>
    public class ParsingResults
    {
        #region Public Properties

        /// <summary>
        /// Gets the parsed values of all command line arguments. The dictionary key is the destination of the argument and the value is the parsed value.
        /// </summary>
        public IDictionary<string, object> ParsedValues { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the name of the command of the parsing results. If a parser has a command, then the arguments of the command are parsed into a separate parsing result. For the top-level parser there is no command name
        /// and this property is set to <c>null</c>.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Gets the sub-results of the a command. If a parser has a command, then the arguments of the command are parsed into these sub results. If no command was parsed, then this property is set to <c>null</c>.
        /// </summary>
        public ParsingResults SubResults { get; private set; }

        /// <summary>
        /// Gets a value that determines whether there are sub-results from a command.
        /// </summary>
        public bool HasSubResults { get => this.SubResults != null; }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Adds a new value to the parsing results. If there already is a value for the specified destination, then the value is overwritten.
        /// </summary>
        /// <param name="destination">The name of the argument.</param>
        /// <param name="value">The parsed value of the argument.</param>
        internal void Add(string destination, object value)
        {
            // Checks if there is already a value for the specified destination, if so it is overwritten, otherwise it is added
            if (this.ParsedValues.ContainsKey(destination))
                this.ParsedValues[destination] = value;
            else
                this.ParsedValues.Add(destination, value);
        }

        /// <summary>
        /// Adds a new value to the parsing results. If there already is a value for the specified destination, then the duplicate resolution policy is used to determine the new value.
        /// </summary>
        /// <param name="destination">The name of the argument.</param>
        /// <param name="value">The parsed value of the argument.</param>
        /// <param name="duplicateResolutionPolicy">A callback, which is used when there already is a value for the specified destination.</param>
        internal void Add(string destination, object value, Func<object, object, object> duplicateResolutionPolicy)
        {
            // Checks if there is already a value for the specified destination, if so the duplicate resolution policy is used to determine the new value
            if (this.ParsedValues.ContainsKey(destination))
                this.ParsedValues[destination] = duplicateResolutionPolicy(this.ParsedValues[destination], value);
            else
                this.ParsedValues.Add(destination, value);
        }

        /// <summary>
        /// Adds a new value to the parsing results. If there already is a value for the specified destination, then the value is overwritten.
        /// </summary>
        /// <param name="argument">The argument that was parsed.</param>
        /// <param name="value">The parsed value of the argument.</param>
        internal void Add(Argument argument, object value)
        {
            if (argument.GetType().GetGenericTypeDefinition() == typeof(NamedArgument<>))
                this.Add(argument.Destination, value, argument.GetType().GetProperty(nameof(NamedArgument<object>.DuplicateResolutionPolicy)).GetValue(argument) as Func<object, object, object>);
            else
                this.Add(argument.Destination, value);
        }

        /// <summary>
        /// Adds new sub-results for the specified command.
        /// </summary>
        /// <param name="name">The name of the command for which the sub-results are to be added.</param>
        /// <returns>Returns the created sub-results.</returns>
        internal ParsingResults AddCommand(string name)
        {
            // Checks if there already are sub-results, if so, an exception is thrown
            if (this.HasSubResults)
                throw new InvalidOperationException("There already is a sub result for a command.");

            // Adds the sub-results for the command and returns them
            this.SubResults = new ParsingResults { Command = name };
            return this.SubResults;
        }

        /// <summary>
        /// Adds new sub-results for the specified command.
        /// </summary>
        /// <param name="command">The command for which the sub-results are to be added.</param>
        /// <returns>Returns the created sub-results.</returns>
        internal ParsingResults AddCommand(Command command) => this.AddCommand(command.Name);

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value of the parsed argument.
        /// </summary>
        /// <param name="destination">The name of the argument.</param>
        /// <exception cref="KeyNotFoundException">If the argument with the specified destination could not be found (i.e. was not parsed), then a <see cref="KeyNotFoundException"/> is thrown.</exception>
        /// <returns>Returns the parsed value of the argument with the specified destination.</returns>
        public object GetParsedValue(string destination)
        {
            // Checks if there is a parsed value for the specified destination, if not, an exception is thrown
            if (!this.ParsedValues.ContainsKey(destination))
                throw new KeyNotFoundException($"There is no parsed value for the destination {destination}.");

            // Returns the parsed value for the specified destination
            return this.ParsedValues[destination];
        }

        /// <summary>
        /// Gets the value of the parsed argument. The value is cast to the specified type.
        /// </summary>
        /// <param name="destination">The name of the argument.</param>
        /// <typeparam name="T">The type into which the value is to be converted.</typeparam>
        /// <exception cref="KeyNotFoundException">If the argument with the specified destination could not be found (i.e. was not parsed), then a <see cref="KeyNotFoundException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the parsed value cannot be cast into the specified type, then a <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the parsed value of the argument with the specified destination.</returns>
        public T GetParsedValue<T>(string destination)
        {
            // Checks if there is a parsed value for the specified destination, if not, an exception is thrown
            if (!this.ParsedValues.ContainsKey(destination))
                throw new KeyNotFoundException($"There is no parsed value for the destination {destination}.");

            // Tries to cast the parsed value into the destination type, if it cannot be cast, then an exception is thrown
            T parsedValue = (T)this.ParsedValues[destination];
            if (parsedValue == null)
                throw new InvalidOperationException($"The parsed value could not be cast into the destination type {parsedValue.GetType().Name}.");

            // Returns the parsed value for the specified destination
            return parsedValue;
        }

        /// <summary>
        /// Gets the value of the parsed argument.
        /// </summary>
        /// <param name="argument">The argument for which the parsed value is to be retrieved.</param>
        /// <exception cref="KeyNotFoundException">If the argument could not be found (i.e. was not parsed), then a <see cref="KeyNotFoundException"/> is thrown.</exception>
        /// <returns>Returns the parsed value of the specified argument.</returns>
        public object GetParsedValue(Argument argument) => this.GetParsedValue(argument.Destination);

        /// <summary>
        /// Gets the value of the parsed argument. The value is cast to the specified type.
        /// </summary>
        /// <param name="argument">The argument for which the parsed value is to be retrieved.</param>
        /// <typeparam name="T">The type into which the value is to be converted.</typeparam>
        /// <exception cref="KeyNotFoundException">If the argument could not be found (i.e. was not parsed), then a <see cref="KeyNotFoundException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If the parsed value cannot be cast into the specified type, then a <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the parsed value of the specified argument.</returns>
        public T GetParsedValue<T>(Argument argument) => this.GetParsedValue<T>(argument.Destination);

        #endregion
    }
}
