
namespace System.CommandLine.Arguments
{
    /// <summary>
    /// Represents a single positional argument of a command line parser. A positional argument, in contrast to a named or flag argument, is an argument, which is only comprised of a value. They are not optional and must
    /// be at the beginning of the command line arguments in exactly the same order as they were declared.
    /// </summary>
    /// <typeparam name="T">The type of the positional argument.</param>
    public class PositionalArgument<T> : Argument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="PositionalArgument"/> instance.
        /// </summary>
        /// <param name="name">The name of the positional argument, which is used in the help string.</param>
        /// <param name="destination">
        /// The name that the positional argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.
        /// </param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name or the destination are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public PositionalArgument(string name, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Stores the arguments for later use
            this.Name = name;
            this.Alias = name;
            this.Destination = destination;
            this.Help = help;
            this.Type = typeof(T);
        }

        #endregion
    }
}
