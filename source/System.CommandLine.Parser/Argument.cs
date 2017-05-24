
#region Using Directives

using System.Reflection;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents a single argument of a command line parser.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</param>
    public class Argument<T> : IArgument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Argument"/> instance.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <param name="duplicateResolutionPolicy">A callback function, which is invoked when the same argument was sepcified twice.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, the destination, the default value, or the duplicate resolution policy are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public Argument(string name, string alias, string destination, string help, T defaultValue, Func<T, T, T> duplicateResolutionPolicy)
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

            // Stores the arguments for later use
            this.Name = name;
            this.Alias = alias;
            this.Destination = destination;
            this.Help = help;
            this.DefaultValue = defaultValue;
            this.DuplicateResolutionPolicy = duplicateResolutionPolicy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the argument, which is used for parsing and in the help string.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the alias name of the argument.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Contains the name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards.
        /// </summary>
        private string destination;

        /// <summary>
        /// Gets the name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards.
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
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get => typeof(T); }

        /// <summary>
        /// Gets the value that the argument receives if it was not detected by the parser.
        /// </summary>
        public T DefaultValue { get; private set; }

        /// <summary>
        /// Gets a callback function, which is invoked when the same argument was sepcified twice.
        /// </summary>
        public Func<T, T, T> DuplicateResolutionPolicy { get; private set; }

        #endregion
    }
}