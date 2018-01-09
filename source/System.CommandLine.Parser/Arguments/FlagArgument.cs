
#region Using Directives

using System.Collections.Generic;
using System.Reflection;

#endregion

namespace System.CommandLine.Arguments
{
    /// <summary>
    /// Represents a single flag argument of a command line parser. In contrast to named arguments, flag arguments, cannot be explicitly assigned a value, but rather get their value from their presence or absence.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the argument. May only be a boolean, integer, or enumeration type. When the type is boolean, then only the presence or absence of the flag is determined. If the type is an integer type (i.e. byte, sbyte,
    /// short, ushort, int, uint, long, or ulong), then the number of occurrences is determined. But if the type is an enumeration type, then the number of occurrences is interpreted as the enumeration value. For example
    /// consider the following enumeration type: <c>enum Severity { None = 0, Low = 1, Medium = 2, High = 3 }</c>, which is the type of the flag argument named "severity" with the alias "s", then the following command line
    /// argument "-sss" would parse to an enumeration value of <c>Severity.High</c>.
    /// </param>
    public class FlagArgument<T> : Argument
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="FlagArgument"/> instance.
        /// </summary>
        /// <param name="name">The name of the argument, which is used for parsing and in the help string.</param>
        /// <param name="alias">The alias name of the argument.</param>
        /// <param name="destination">The name that the argument will have in the result dictionary after parsing. This should adhere to normal C# naming standards. If it does not, it is automatically converted.</param>
        /// <param name="help">A descriptive help text for the argument, which is used in the help string.</param>
        /// <exception cref="ArgumentNullException">If either the name, the alias, or the destination are <c>null</c>, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        public FlagArgument(string name, string alias, string destination, string help)
        {
            // Validates the arguments
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException(nameof(destination));

            // Stores the arguments for later use
            this.Name = name;
            this.Alias = alias;
            this.Destination = destination;
            this.Help = help;
            this.Type = typeof(T);

            // Checks if the type of the flag argument is allowed
            if (!FlagArgument<T>.allowedTypes.Contains(this.Type) || this.Type.GetTypeInfo().IsEnum)
                throw new ArgumentException("The specified type is not allowed for flag arguments.", nameof(T));
        }

        #endregion

        #region Private Static Fields

        /// <summary>
        /// Contains a list of all the types that are allowed to be used with <see cref="FlagArgument"/>.
        /// </summary>
        private static List<Type> allowedTypes = new List<Type> { typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };

        #endregion
    }
}
