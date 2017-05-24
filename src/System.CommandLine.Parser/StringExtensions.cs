
#region Using Directives

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents a set of extension methods for the <see cref="System.String"/> class.
    /// </summary>
    internal static class StringExtensions
    {
        #region Extension Methods

        /// <summary>
        /// Converts an arbitrary string into a valid C# property name in camel case.
        /// </summary>
        /// <param name="name">The string that is to be converted into a valid C# property name.</param>
        /// <exception cref="ArgumentNullException">If the specified string is either <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</excpetion>
        /// <returns>Returns a valid C# property name in camel case.</returns>
        public static string ToCamelCasePropertyName(this string name)
        {
            // Validates the parameters
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            // Makes sure that the name is a valid C# name by first removing all characters that are not allowed, except for whitespaces, underscores, and dashes (they are needed to camel case the name)
            name = Regex.Replace(name, @"[^0-9a-zA-Z \-_]", string.Empty).Trim();

            // Camel cases the name
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string part in name.Split(' ', '-', '_'))
            {
                if (part.Length == 0)
                    continue;
                stringBuilder.Append(part.Substring(0, 1).ToUpperInvariant());
                stringBuilder.Append(part.Substring(1).ToLowerInvariant());
            }
            name = stringBuilder.ToString();

            // Makes sure that the first character of the name is a letter
            if (new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }.Contains(name.Substring(0, 1)))
                name = $"_{name}";

            // Returns the camel cased name
            return name;
        }

        #endregion
    }
}