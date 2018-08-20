
#region Using Directives

using System.CommandLine.ValueConverters;
using System.Reflection;

#endregion

namespace System.CommandLine.Arguments
{
    /// <summary>
    /// Represents a single named argument of a command line parser. A named argument is a command line argument that is comprised of a name and an explicit value.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</param>
    public class NamedArgument<T> : Argument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="NamedArgument"/> instance.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <param name="defaultValue">The value that the argument receives if it was not detected by the parser.</param>
        /// <param name="duplicateResolutionPolicy">A callback function, which is invoked when the same argument was specified more than once.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination are <c>null</c>, empty, or only consist of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public NamedArgument(string name, string alias, string destination, string help, T defaultValue, Func<T, T, T> duplicateResolutionPolicy)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Stores the arguments for later use
            this.Name = name;
            this.Alias = alias;
            this.Destination = destination;
            this.Help = help;
            this.DefaultValue = defaultValue;
            this.DuplicateResolutionPolicy = duplicateResolutionPolicy ?? this.ResolveDuplicateValues;
            this.Type = typeof(T);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value that the argument receives if it was not detected by the parser.
        /// </summary>
        public T DefaultValue { get; private set; }

        /// <summary>
        /// Gets a callback function, which is invoked when the same argument was specified more than once.
        /// </summary>
        public Func<T, T, T> DuplicateResolutionPolicy { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// This the default duplicate resolution strategy. If the type parameter <see cref="T"/> is a supported collection type, then the old and the new value are merged into a single collection, otherwise the new value
        /// always wins.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>Returns the resolved value.</returns>
        private T ResolveDuplicateValues(T oldValue, T newValue)
        {
            if (CollectionHelper.IsSupportedCollectionType(typeof(T)))
                return (T)CollectionHelper.Merge(typeof(T), oldValue, newValue);
            return newValue;
        }

        #endregion
    }
}
