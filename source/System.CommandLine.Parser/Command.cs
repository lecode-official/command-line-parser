
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
    /// Represents a command. A command is a sub-parser, which allows specialized operations to be performed.
    /// </summary>
    public class Command
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="subParser">The sub-parser that is responsible for parsing the arguments of the command.</param>
        public Command(string name, string alias, Parser subParser)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (subParser == null)
                throw new ArgumentNullException(nameof(subParser));

            // Stores the arguments for later use
            this.Name = name;
            this.Alias = alias;
            this.SubParser = subParser;
        }

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="subParser">The sub-parser that is responsible for parsing the arguments of the command.</param>
        public Command(string name, Parser subParser)
            : this(name, null, subParser)
        {}

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="description">A descriptive help text for the command, which is used in the help string.</param>
        /// <param name="parserOptions">The options of the sub-parser that is responsible for parsing the arguments of the command.<param>
        public Command(string name, string alias, string description, ParserOptions parserOptions)
            : this(name, alias, new Parser(description, parserOptions))
        {}

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="alias">The alias of the command, which can be used as an alternative to the name.</param>
        /// <param name="description">A descriptive help text for the command, which is used in the help string.</param>
        public Command(string name, string alias, string description)
            : this(name, alias, description, ParserOptions.Default)
        {}

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">A descriptive help text for the command, which is used in the help string.</param>
        public Command(string name, string description)
            : this(name, null, description, ParserOptions.Default)
        {}

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="parserOptions">The options of the sub-parser that is responsible for parsing the arguments of the command.<param>
        public Command(string name, ParserOptions parserOptions)
            : this(name, null, null, parserOptions)
        {}

        /// <summary>
        /// Initializes a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        public Command(string name)
            : this(name, null, null, ParserOptions.Default)
        {}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the alias of the command. An alias can be used as an alternative to the name of the command.
        /// </summary>
        public string Alias { get; protected set; }

        /// <summary>
        /// Gets or sets the sub-parser that is responsible for parsing the arguments of the command.
        /// </summary>
        public Parser SubParser { get; protected set; }

        #endregion
    }
}
