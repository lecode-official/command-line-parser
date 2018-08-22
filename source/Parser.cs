
#region Using Directives

using System.Collections.Generic;
using System.CommandLine.Arguments;
using System.CommandLine.ValueConverters;
using System.Linq;

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
            : this(null, new ParserOptions())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="description">The description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.</param>
        public Parser(string description)
            : this(description, new ParserOptions())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="options">The options of the command line parser.</param>
        /// <exception cref="ArgumentNullException">If the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public Parser(ParserOptions options)
            : this(null, options)
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
            this.Description = description;
            this.Options = options;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the description of the parser. If this is a root parser, then this is the description of the application. Otherwise this it the description for the command.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the positional arguments of the parser.
        /// </summary>
        public IEnumerable<Argument> PositionalArguments { get; private set; } = new List<Argument>();

        /// <summary>
        /// Gets the named arguments of the parser.
        /// </summary>
        public IEnumerable<Argument> NamedArguments { get; private set; } = new List<Argument>();

        /// <summary>
        /// Gets the flag arguments of the parser.
        /// </summary>
        public IEnumerable<Argument> FlagArguments { get; private set; } = new List<Argument>();

        /// <summary>
        /// Gets the commands of the parser (which in turn consist of a sub-parser, which is able to parse the arguments of the command).
        /// </summary>
        public IEnumerable<Command> Commands { get; private set; } = new List<Command>();

        /// <summary>
        /// Gets or sets the options of the command line parser.
        /// </summary>
        public ParserOptions Options { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the specified argument has a valid name and whether another argument with the same name or alias already exists and throws an exception if so.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If the name or alias of the argument is invalid or there already is a named argument, a flag argument, or a positional argument with the same name or alias as the specified argument, then an
        /// <see cref="InvalidOperationException"/> is thrown.
        /// </exception>
        private void AssertArgumentIsValidAndUnique(Argument argument)
        {
            // Checks if the name of the argument starts with one of the argument prefixes or contains the key value separator, if so, then an exception is thrown
            if (argument.Name.StartsWith(this.Options.ArgumentPrefix))
                throw new InvalidOperationException($"The name of the argument {argument.Name} must not start with the argument prefix {this.Options.ArgumentPrefix}.");
            if (!string.IsNullOrWhiteSpace(argument.Alias) && argument.Alias.StartsWith(this.Options.ArgumentPrefix))
                throw new InvalidOperationException($"The alias of the argument {argument.Alias} must not start with the argument prefix {this.Options.ArgumentAliasPrefix}.");
            if (argument.Name.Contains(this.Options.KeyValueSeparator))
                throw new InvalidOperationException($"The name of the argument {argument.Name} must not contain the key value separator {this.Options.KeyValueSeparator}.");
            if (!string.IsNullOrWhiteSpace(argument.Alias) && argument.Alias.Contains(this.Options.KeyValueSeparator))
                throw new InvalidOperationException($"The alias of the argument {argument.Alias} must not contain the key value separator {this.Options.KeyValueSeparator}.");

            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there are any arguments with the same name
            if (this.NamedArguments.Any(namedArgument => string.Equals(namedArgument.Name, argument.Name, stringComparison)))
                throw new InvalidOperationException($"There already is a named argument with the name {argument.Name}.");
            if (this.FlagArguments.Any(flagArgument => string.Equals(flagArgument.Name, argument.Name, stringComparison)))
                throw new InvalidOperationException($"There already is a flag argument with the name {argument.Name}.");
            if (this.PositionalArguments.Any(positionalArgument => string.Equals(positionalArgument.Name, argument.Name, stringComparison)))
                throw new InvalidOperationException($"There already is a positional argument with the name {argument.Name}.");

            // Checks if there are any arguments with the same destination
            if (this.NamedArguments.Any(namedArgument => string.Equals(namedArgument.Destination, argument.Destination, stringComparison)))
                throw new InvalidOperationException($"There already is a named argument with the destination {argument.Destination}.");
            if (this.FlagArguments.Any(flagArgument => string.Equals(flagArgument.Destination, argument.Destination, stringComparison)))
                throw new InvalidOperationException($"There already is a flag argument with the destination {argument.Destination}.");
            if (this.PositionalArguments.Any(positionalArgument => string.Equals(positionalArgument.Destination, argument.Destination, stringComparison)))
                throw new InvalidOperationException($"There already is a positional argument with the destination {argument.Destination}.");

            // Checks if there are any other arguments with the same alias
            if (!string.IsNullOrWhiteSpace(argument.Alias))
            {
                if (this.NamedArguments.Any(namedArgument => string.Equals(namedArgument.Alias, argument.Alias, stringComparison)))
                    throw new InvalidOperationException($"There already is a named argument with the alias {argument.Alias}.");
                if (this.FlagArguments.Any(flagArgument => string.Equals(flagArgument.Alias, argument.Alias, stringComparison)))
                    throw new InvalidOperationException($"There already is a flag argument with the alias {argument.Alias}.");
            }
        }

        /// <summary>
        /// Parses the token and checks if it references a named argument.
        /// </summary>
        /// <param name="token">The token that is to be parsed.</param>
        /// <param name="namedArgument">
        /// This out parameter will be assigned the named argument which was referenced by the token. If the token does not match any named argument then this out parameter will be set to <c>null</c>.
        /// </param>
        /// <returns>Returns <c>true</c> if the token references a named argument and <c>false</c> otherwise.</returns>
        private bool ParseNamedArgument(string token, out Argument namedArgument, out string namedArgumentValue)
        {
            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // If the named argument could not be parsed, then all out parameters are set to null
            namedArgument = null;
            namedArgumentValue = null;

            // For named arguments the token may contain not only the argument reference, but the value as well (e.g. "--age=18"), so the value has to be separated out
            string namedArgumentReference = token;
            string value = null;
            if (namedArgumentReference.IndexOf(this.Options.KeyValueSeparator) > 0)
            {
                string[] tokenElements = token.Split(new string[] { this.Options.KeyValueSeparator }, StringSplitOptions.None);
                if (tokenElements.Length != 2)
                    return false;
                namedArgumentReference = tokenElements[0];
                value = tokenElements[1];
            }

            // Checks if there is a named argument with the name or alias that is specified in the token
            if (this.NamedArguments.Any(argument => namedArgumentReference.Equals(string.Concat(this.Options.ArgumentPrefix, argument.Name), stringComparison)) ||
                this.NamedArguments.Any(argument => namedArgumentReference.Equals(string.Concat(this.Options.ArgumentAliasPrefix, argument.Alias), stringComparison)))
            {
                namedArgument = this.NamedArguments.Single(argument =>
                    namedArgumentReference.Equals(string.Concat(this.Options.ArgumentPrefix, argument.Name), stringComparison) ||
                    namedArgumentReference.Equals(string.Concat(this.Options.ArgumentAliasPrefix, argument.Alias), stringComparison)
                );
                namedArgumentValue = value;
                return true;
            }

            // Since the token neither matches the name of a named argument nor an alias of a named argument, the token does not represent a named argument
            return false;
        }

        /// <summary>
        /// Parses the specified token and checks if it references a flag argument.
        /// </summary>
        /// <param name="token">The token that is to be parsed.</param>
        /// <param name="flagArgument">
        /// This out parameter will be assigned the flag argument which was referenced by the token. If the token does not match any flag argument then this out parameter will be set to <c>null</c>.
        /// </param>
        /// <returns>Returns <c>true</c> if the token references a flag argument and <c>false</c> otherwise.</returns>
        private bool ParseFlagArgument(string token, out Argument flagArgument)
        {
            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there is a flag argument with the name or alias that is specified in the token
            if (this.FlagArguments.Any(argument => token.Equals(string.Concat(this.Options.ArgumentPrefix, argument.Name), stringComparison)) ||
                this.FlagArguments.Any(argument => token.Equals(string.Concat(this.Options.ArgumentAliasPrefix, argument.Alias), stringComparison)))
            {
                flagArgument = this.FlagArguments.Single(argument =>
                    token.Equals(string.Concat(this.Options.ArgumentPrefix, argument.Name), stringComparison) ||
                    token.Equals(string.Concat(this.Options.ArgumentAliasPrefix, argument.Alias), stringComparison)
                );
                return true;
            }

            // Since the token neither matches the name of a flag argument nor an alias of a flag argument, the token does not represent a flag argument
            flagArgument = null;
            return false;
        }

        /// <summary>
        /// Parses the specified token and checks if it is a multi-character flag argument and references multiple flag arguments.
        /// </summary>
        /// <param name="token">The token that is to be parsed.</param>
        /// <param name="command">
        /// This out parameter will be assigned a dictionary with all the flag arguments and the number of their occurrences that were referenced by the token. If the token is not a valid multi-character flag argument,
        /// then this out parameter will be set to <c>null</c>.
        /// </param>
        /// <returns>Returns <c>true</c> if the token is a multi-character flag argument and <c>false</c> otherwise.</returns>
        private bool ParseMultiCharacterFlagArgument(string token, out IDictionary<Argument, ulong> flagArgumentOccurrences)
        {
            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if the token starts with the argument alias prefix, if not, then the token cannot reference a multi-character flag
            flagArgumentOccurrences = null;
            if (!token.StartsWith(this.Options.ArgumentAliasPrefix))
                return false;

            // Cycles through all flags and tries to match them with flag arguments
            Dictionary<Argument, ulong> occurrences = new Dictionary<Argument, ulong>();
            foreach (string flag in token.Replace(this.Options.ArgumentAliasPrefix, string.Empty).Select(character => character.ToString()))
            {
                // Gets the corresponding argument for the flag
                Argument flagArgument = this.FlagArguments.FirstOrDefault(argument => argument.Alias.Equals(flag, stringComparison));

                // If no flag argument could be found, then the multi-character flag argument does not match, at least not entirely, and is therefore invalid
                if (flagArgument == null)
                    return false;

                if (occurrences.ContainsKey(flagArgument))
                    occurrences[flagArgument] += 1;
                else
                    occurrences.Add(flagArgument, 1);
            }

            // Since all flags in the multi-character flag were matched with a flag argument, the multi-character flag argument is valid
            flagArgumentOccurrences = occurrences;
            return true;
        }

        /// <summary>
        /// Parses the specified token and checks if it references a command.
        /// </summary>
        /// <param name="token">The token that is to be parsed.</param>
        /// <param name="command">This out parameter will be assigned the command which was referenced by the token. If the token does not match any command, then this out parameter will be set to <c>null</c>.</param>
        /// <returns>Returns <c>true</c> if the token references a command and <c>false</c> otherwise.</returns>
        private bool ParseCommand(string token, out Command command)
        {
            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there is a command with the name or alias that is specified in the token
            if (this.Commands.Any(c => token.Equals(c.Name, stringComparison)) || this.Commands.Any(c => token.Equals(c.Alias, stringComparison)))
            {
                command = this.Commands.Single(c => token.Equals(c.Name, stringComparison) || token.Equals(c.Alias, stringComparison));
                return true;
            }

            // Since the token neither matches the name of a command nor an alias of a command, the token does not represent a command
            command = null;
            return false;
        }

        /// <summary>
        /// Determines if the specified token references a named argument, a flag argument, a multi-character flag argument, or a command.
        /// </summary>
        /// <param name="token">The token that is to be parsed.</param>
        /// <returns>Returns <c>true</c> if the token references a named argument, a flag argument, a multi-character flag argument, or a command and <c>false</c> otherwise.</returns>
        public bool IsArgumentOrCommand(string token)
        {
            // Checks if the token references a named argument, a flag argument, a multi-character flag argument, or a command, if so, true is returned
            if (this.ParseNamedArgument(token, out Argument _, out string _))
                return true;
            if (this.ParseFlagArgument(token, out Argument _))
                return true;
            if (this.ParseMultiCharacterFlagArgument(token, out IDictionary<Argument, ulong> _))
                return true;
            if (this.ParseCommand(token, out Command _))
                return true;

            // Since the neither references a named argument, a flag argument, a multi-character flag argument, nor a command, false is returned
            return false;
        }

        /// <summary>
        /// Parses the command line arguments by matching them to the declared arguments and commands.
        /// </summary>
        /// <param name="tokenQueue">A queue, which contains the tokens.</param>
        /// <returns>Returns the parsing results, which is a bag of arguments.</returns>
        private ParsingResults Parse(Queue<string> tokenQueue)
        {
            // Creates new parsing results, which will hold the parsed argument values
            ParsingResults parsingResults = new ParsingResults();

            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // At first the positional arguments have to be parsed, because they are non-optional and have to be there
            foreach (Argument positionalArgument in this.PositionalArguments)
            {
                // Checks if there are still enough tokens left in the command line arguments
                if (!tokenQueue.Any())
                    throw new InvalidOperationException($"The non-optional positional argument {positionalArgument.Name} is missing.");

                // Adds the value of the positional argument to the parsing results
                parsingResults.Add(positionalArgument, ValueConverter.Convert(positionalArgument.Type, tokenQueue.Dequeue()));
            }

            // Named arguments have default values, so a list is needed to keep track of all the named arguments that have not been parsed
            List<Argument> missingNamedArguments = this.NamedArguments.ToList();

            // Since flag arguments get their value from being present or absent, all flag values get a value (missing flags just get their "absent" value, e.g. false), there number of occurrences have to be recorded
            Dictionary<Argument, ulong> flagArgumentOccurrences = new Dictionary<Argument, ulong>();
            foreach (Argument flagArgument in this.FlagArguments)
                flagArgumentOccurrences.Add(flagArgument, 0);

            // While there are still token in the queue, the named arguments, flag arguments, and commands have to be parsed
            while (tokenQueue.Any())
            {
                // Checks if the next token references a named argument, a flag argument, a multi-character flag argument, a command, or is an invalid token
                string nextToken = tokenQueue.Dequeue();
                if (this.ParseNamedArgument(nextToken, out Argument namedArgument, out string namedArgumentValue))
                {
                    // Named arguments may have more than one value (which is helpful when dealing with lists or custom duplicate resolution policies), therefore the next tokens are interpreted as values as long as
                    // they do not reference any arguments or commands (but at least the first token after the named parameter is used as a value)
                    if (namedArgumentValue != null)
                        parsingResults.Add(namedArgument, ValueConverter.Convert(namedArgument.Type, namedArgumentValue));
                    else
                        parsingResults.Add(namedArgument, ValueConverter.Convert(namedArgument.Type, tokenQueue.Dequeue()));
                    while (tokenQueue.Any() && !this.IsArgumentOrCommand(tokenQueue.Peek()))
                        parsingResults.Add(namedArgument, ValueConverter.Convert(namedArgument.Type, tokenQueue.Dequeue()));

                    // Since the named argument was parsed it is not missing from the command line arguments and the default value does not need to be set
                    missingNamedArguments.Remove(namedArgument);
                }
                else if (this.ParseFlagArgument(nextToken, out Argument flagArgument))
                {
                    flagArgumentOccurrences[flagArgument] += 1;
                }
                else if (this.ParseMultiCharacterFlagArgument(nextToken, out IDictionary<Argument, ulong> occurrences))
                {
                    foreach (Argument argument in occurrences.Keys)
                        flagArgumentOccurrences[argument] += occurrences[argument];
                }
                else if (this.ParseCommand(nextToken, out Command command))
                {
                    ParsingResults subResults = command.SubParser.Parse(tokenQueue);
                    parsingResults.AddCommand(command, subResults);
                    break;
                }
                else
                {
                    // Since the next token in the queue is neither a named argument or flag argument, nor a command, the token is erroneous and an exception is thrown
                    throw new InvalidOperationException($"Unexpected token {nextToken}. This token is neither an argument nor a command.");
                }
            }

            // Adds the default values of all named arguments that were not in the command line arguments
            foreach (Argument namedArgument in missingNamedArguments)
            {
                Type namedArgumentType = namedArgument.GetType();
                if (namedArgumentType.GetGenericTypeDefinition() != typeof(NamedArgument<>))
                    continue;
                object namedArgumentDefaultValue = namedArgumentType.GetProperty(nameof(NamedArgument<object>.DefaultValue)).GetValue(namedArgument);
                parsingResults.Add(namedArgument, namedArgumentDefaultValue);
            }

            // Adds the value of the flag arguments
            foreach (Argument flagArgument in flagArgumentOccurrences.Keys)
                parsingResults.Add(flagArgument, ValueConverter.Convert(flagArgument.Type, flagArgumentOccurrences[flagArgument].ToString()));

            // Returns the result of the parsing
            return parsingResults;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddPositionalArgument<T>(string name) => this.AddPositionalArgument<T>(name, name, null);

        /// <summary>
        /// Adds a positional argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
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
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the positional argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddPositionalArgument<T>(string name, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already an argument with the same name
            PositionalArgument<T> newPositionalArgument = new PositionalArgument<T>(name, destination, help);
            this.AssertArgumentIsValidAndUnique(newPositionalArgument);

            // Adds the positional argument to the parser
            (this.PositionalArguments as List<Argument>).Add(newPositionalArgument);

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name) => this.AddNamedArgument<T>(name, null, name, null, default(T), null);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias) => this.AddNamedArgument<T>(name, alias, name, null, default(T), null);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string help) => this.AddNamedArgument<T>(name, alias, name, help, default(T), null);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string destination, string help) => this.AddNamedArgument<T>(name, alias, destination, help, default(T), null);

        /// <summary>
        /// Adds a named argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination are <c>null</c>, empty, or only consist of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
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
        /// <exception cref="ArgumentNullException">If either the name or the destination are <c>null</c>, empty, or only consist of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddNamedArgument<T>(string name, string alias, string destination, string help, T defaultValue, Func<T, T, T> duplicateResolutionPolicy)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already an argument with the same name or alias
            NamedArgument<T> newNamedArgument = new NamedArgument<T>(name, alias, destination, help, defaultValue, duplicateResolutionPolicy);
            this.AssertArgumentIsValidAndUnique(newNamedArgument);

            // Adds the argument to the parser
            (this.NamedArguments as List<Argument>).Add(newNamedArgument);

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name) => this.AddFlagArgument<T>(name, null, name, null);

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias) => this.AddFlagArgument<T>(name, alias, name, null);

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias, string help) => this.AddFlagArgument<T>(name, alias, name, help);

        /// <summary>
        /// Adds a flag argument to the command line parser.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination are <c>null</c>, empty, or only consist of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is an argument with the same name or the same alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <returns>Returns this command line parser so that method invocations can be chained.</returns>
        public Parser AddFlagArgument<T>(string name, string alias, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Checks if there is already an argument with the same name or alias
            FlagArgument<T> newFlagArgument = new FlagArgument<T>(name, alias, destination, help);
            this.AssertArgumentIsValidAndUnique(newFlagArgument);

            // Adds the argument to the parser
            (this.FlagArguments as List<Argument>).Add(newFlagArgument);

            // Returns this parser, so that method invocations can be chained
            return this;
        }

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name) => this.AddCommand(name, null, null, this.Options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="parserOptions">The parser options for the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists or white spaces or the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, ParserOptions parserOptions) => this.AddCommand(name, null, null, parserOptions);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description for the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string description) => this.AddCommand(name, null, description, this.Options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="description">The description for the command.</param>
        /// <param name="parserOptions">The parser options for the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces or the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string alias, string description) => this.AddCommand(name, alias, description, this.Options);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="description">The description for the command.</param>
        /// <param name="parserOptions">The parser options for the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces or the options are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name or alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string alias, string description, ParserOptions parserOptions)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (parserOptions == null)
                throw new ArgumentNullException(nameof(parserOptions));

            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there is already a command with the same name or alias
            if (this.Commands.Any(command => string.Equals(command.Name, name, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the name {name}.");
            if (!string.IsNullOrWhiteSpace(alias) && this.Commands.Any(command => string.Equals(command.Alias, alias, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the alias {alias}.");

            // Creates the parser for the new command and adds it to the list of commands
            Command newCommand = new Command(name, alias, description, parserOptions);
            (this.Commands as List<Command>).Add(newCommand);

            // Returns the created parser
            return newCommand.SubParser;
        }

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="subParser">The parser that is used to parse the arguments of the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces or the parser is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, Parser subParser) => this.AddCommand(name, null, subParser);

        /// <summary>
        /// Creates a new sub-command for the command line parser.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="subParser">The parser that is used to parse the arguments of the command.</param>
        /// <exception cref="ArgumentNullException">If the name is <c>null</c>, empty, or only consists of white spaces or the parser is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name or alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the argument parser for the command, which can then be configured.</returns>
        public Parser AddCommand(string name, string alias, Parser subParser)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (subParser == null)
                throw new ArgumentNullException(nameof(subParser));

            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there is already a command with the same name or alias
            if (this.Commands.Any(command => string.Equals(command.Name, name, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the name {name}.");
            if (!string.IsNullOrWhiteSpace(alias) && this.Commands.Any(command => string.Equals(command.Alias, alias, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the alias {alias}.");

            // Adds the new command to the list of commands
            Command newCommand = new Command(name, alias, subParser);
            (this.Commands as List<Command>).Add(newCommand);

            // Returns the sub-parser
            return subParser;
        }

        /// <summary>
        /// Adds the specified command to the command line parser.
        /// </summary>
        /// <param name="command">The command that is to be added.</param>
        /// <exception cref="ArgumentNullException">If the command is <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="InvalidOperationException">If there already is a command with the same name or alias, then an <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>Returns the parser of the command, which can be used to chain calls.</returns>
        public Parser AddCommand(Command command)
        {
            // Validates the arguments
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // Determines the string comparison type based on whether the casing should be ignored or not
            StringComparison stringComparison = this.Options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Checks if there is already a command with the same name or alias
            if (this.Commands.Any(otherCommand => string.Equals(otherCommand.Name, command.Name, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the name {command.Name}.");
            if (!string.IsNullOrWhiteSpace(command.Alias) && this.Commands.Any(otherCommand => string.Equals(otherCommand.Alias, command.Alias, stringComparison)))
                throw new InvalidOperationException($"There already is a command with the alias {command.Alias}.");

            // Adds the command to the list of commands
            (this.Commands as List<Command>).Add(command);

            // Returns the sub-parser
            return command.SubParser;
        }

        /// <summary>
        /// Parses the command line arguments by matching them to the declared arguments and commands.
        /// </summary>
        /// <param name="commandLineArguments">The command line arguments that were retrieved by the application.</param>
        /// <returns>Returns the parsing results, which is a bag of arguments.</returns>
        public ParsingResults Parse(string[] commandLineArguments)
        {
            // Copies the command line arguments into a queue so that they are easier to parse without having to do fancy indexing, the first token is dequeued right away, because it is the file name of the executable
            Queue<string> tokenQueue = new Queue<string>(commandLineArguments);
            tokenQueue.Dequeue();

            // Parses the command line arguments and returns the result
            return this.Parse(tokenQueue);
        }

        #endregion
    }
}
