
namespace System.CommandLine
{
    /// <summary>
    /// Represents the base interface all argument types have to implement.
    /// </summary>
    public interface IArgument
    {
        #region Properties

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the alias name of the argument.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets the name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards.
        /// </summary>
        string Destination { get; }

        /// <summary>
        /// Gets the descriptive help text for the argument, which is used in the help string.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        Type Type { get; }

        #endregion
    }
}