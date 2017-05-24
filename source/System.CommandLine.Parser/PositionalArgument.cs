
namespace System.CommandLine
{
    /// <summary>
    /// Represents a single positional argument of a command line parser.
    /// </summary>
    public class PositionalArgument : IArgument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="PositionalArgument"/> instance.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="destination">The name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="type">The type of the positional argument.</param>
        /// <exception cref="ArgumentNullException">If either the name, the destination, or the type are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public PositionalArgument(string name, string destination, string help, Type type)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Stores the arguments for later use
            this.Name = name;
            this.Destination = destination;
            this.Help = help;
            this.Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the positional argument, which is used in the help string.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Contains the name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards.
        /// </summary>
        private string destination;

        /// <summary>
        /// Gets the name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards.
        /// </summary>
        public string Destination
        {
            get => this.destination;
            set => this.destination = value.ToCamelCasePropertyName();
        }

        /// <summary>
        /// Gets the descriptive help text for the argument, which is used in the help string.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// Gets the type of the positional argument.
        /// </summary>
        public Type Type { get; private set; }

        #endregion
    }
}