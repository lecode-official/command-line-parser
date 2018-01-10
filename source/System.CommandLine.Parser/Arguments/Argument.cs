
namespace System.CommandLine.Arguments
{
    /// <summary>
    /// Represents the abstract base class that all argument types have to implement.
    /// </summary>
    public abstract class Argument
    {
        #region Properties

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the alias name of the argument.
        /// </summary>
        public string Alias { get; protected set; }

        /// <summary>
        /// Contains the name that the argument will have in the result dictionary after parsing. This must adhere to normal C# naming standards.
        /// </summary>
        private string destination;

        /// <summary>
        /// Gets the name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.
        /// </summary>
        public string Destination
        {
            get => this.destination;
            protected set => this.destination = value.ToCamelCasePropertyName();
        }

        /// <summary>
        /// Gets the descriptive help text for the argument, which is used in the help string.
        /// </summary>
        public string Help { get; protected set; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get; protected set; }

        #endregion
    }
}
