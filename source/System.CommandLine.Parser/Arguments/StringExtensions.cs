
#region Using Directives

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace System.CommandLine.Arguments
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
        /// <exception cref="ArgumentNullException">If the specified string is either <c>null</c>, empty, or only consists of white spaces, then an <see cref="ArgumentNullException"/> is thrown.</exception>
        /// <returns>Returns a valid C# property name in camel case.</returns>
        public static string ToCamelCasePropertyName(this string name)
        {
            // Validates the parameters
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            // Makes sure that the name is a valid C# name by first removing all characters that are not allowed, except for whitespaces, underscores, and dashes (they are needed to camel case the name)
            const string firstCharacterRegex = @"\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}";
            const string extendedCharacterRegex = @"\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}";
            Regex invalidCharacters = new Regex(string.Format(@"[^{0}{1} \-_]", firstCharacterRegex, extendedCharacterRegex));
            name = invalidCharacters.Replace(name, string.Empty);

            // Adds spaces at the end of strings of digits, to make sure that the following characters are correctly capitalized
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < name.Length; index++)
            {
                char currentCharacter = name[index];
                stringBuilder.Append(currentCharacter);
                if (index == name.Length - 1)
                    break;
                char nextCharacter = name[index + 1];
                if (char.IsDigit(currentCharacter) && !char.IsDigit(nextCharacter))
                    stringBuilder.Append(" ");
            }
            name = stringBuilder.ToString();

            // Camel cases the name
            stringBuilder = new StringBuilder();
            foreach (string part in name.Split(' ', '-', '_'))
            {
                if (part.Length == 0)
                    continue;
                stringBuilder.Append(part.Substring(0, 1).ToUpperInvariant());
                stringBuilder.Append(part.Substring(1));
            }
            name = stringBuilder.ToString();

            // Makes sure that the first character of the name is a letter
            if (char.IsDigit(name[0]))
                return $"_{name}";
            return name;
        }

        #endregion
    }
}
