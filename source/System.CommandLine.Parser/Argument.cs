
#region Using Directives

using System.Reflection;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents a single argument of a command line parser.
    /// </summary>
    public class Argument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Argument"/> instance.
        /// </summary>
        /// <param name="name">The name of the argument, which is used in the help string.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="type">The type of the argument.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <param name="duplicateResolutionPolicy">A callback function, which is invoked when the same argument was sepcified twice.</param>
        /// <exception cref="ArgumentNullException">If either the name, the destination, the type, the default value, or the duplicate resolution policy are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <exception cref="ArgumentException">If the default value is not of the specified type or does not derive from the specified type, then an <see cref="ArgumentException"/> is thrown.</exception>
        public Argument(string name, string destination, string help, Type type, object defaultValue, Func<object, object, object> duplicateResolutionPolicy)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (defaultValue == null)
                throw new ArgumentNullException(nameof(defaultValue));
            if (!type.GetTypeInfo().IsAssignableFrom(defaultValue.GetType().GetTypeInfo()))
                throw new ArgumentException("The default value is not of the specified type or derived from the specified type.");
            if (duplicateResolutionPolicy == null)
                throw new ArgumentNullException(nameof(duplicateResolutionPolicy));

            // Stores the arguments for later use
            this.Name = name;
            this.Destination = destination;
            this.Help = help;
            this.Type = type;
            this.DefaultValue = defaultValue;
            this.DuplicateResolutionPolicy = duplicateResolutionPolicy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the argument, which is used in the help string.
        /// </summary>
        public string Name { get; private set; }

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
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the value that the argument receives if it was not detected by the parser.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets a callback function, which is invoked when the same argument was sepcified twice.
        /// </summary>
        public Func<object, object, object> DuplicateResolutionPolicy { get; private set; }

        #endregion
    }
}