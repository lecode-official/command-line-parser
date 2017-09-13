
#region Using Directives

using System.Runtime.InteropServices;

#endregion

namespace System.CommandLine
{
    /// <summary>
    /// Represents a property bag for the different options for the command line parser.
    /// </summary>
    public class ParserOptions
    {
        #region Public Static Properties

        /// <summary>
        /// Contains some sensible default options for the command line parser.
        /// </summary>
        private static ParserOptions defaultOptions = new ParserOptions
        {
            IgnoreCase = true,
            ArgumentPrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/" : "--",
            ArgumentAliasPrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/" : "-",
            AllowMultiCharacterFlags = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        };

        /// <summary>
        /// Gets some sensible default options for the command line parser. The default options are operating-system-aware, so for example <see cref="NamedArgumentPrefix"/> is set to "/" when being used under Windows,
        /// but set to "--" when used under Linux or macOS.
        /// </summary>
        public static ParserOptions Default
        {
            get => ParserOptions.defaultOptions;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that determines whether the parser ignores case or not when parsing argument names.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets the prefix that is used for arguments. Under UNIX-like operating systems, this is usually two hyphen-minuses or dash characters: "--". Under Windows this is usually a single forward slash "/".
        /// </summary>
        public string ArgumentPrefix { get; set; }

        /// <summary>
        /// Gets or sets the prefix that is used for the aliases of arguments. Under UNIX-like operating systems, this is usually a single hyphen-minus or dash character: "-". Under Windows this is usually a single
        /// forward slash "/".
        /// </summary>
        public string ArgumentAliasPrefix { get; set; }

        /// <summary>
        /// Contains a value that determines whether multiple flags can be combined into a single flag.
        /// </summary>
        private bool allowMultiCharacterFlags;

        /// <summary>
        /// Gets or sets a value that determines whether multiple flags arguments can be combined into a single flag. For example if there are two flag arguments "flag1" and "flag2" and there aliases are "a" and "b"
        /// respectively, then the following notation is valid for setting both flags at once "-ab" (equivalent to "-ba") if <see cref="AllowMultiCharacterFlags"/> is set to true. This is just a short-hand notation
        /// for the equivalent "-a -b". This notation is commmon in UNIX-like operating systems. Multi-character flags are only allowed if the prefix for arguments is not the same as the prefix for the aliases of
        /// arguments. For example in Windows these prefixes are usually the both a single forward slash "/". In this case multi-character flags may be mistaken for other named or flag arguments and are therefore
        /// not allowed. Consider the case where there are three flag arguments "ab", "foo", and "bar", where "foo" and "bar" have the aliases "a" and "b" respectively. In this case the following command line
        /// argument "/ab" may either be interpreted as "ab", or "foo" and "bar".
        /// </summary>
        public bool AllowMultiCharacterFlags
        {
            get => this.allowMultiCharacterFlags;
            set
            {
                if (value && this.ArgumentPrefix == this.ArgumentAliasPrefix)
                    throw new ArgumentException("If the same prefix is used for arguments and for the aliases of arguments, then multi-character flags may be ambigous and are therefore not allowed.", nameof(value));
                this.allowMultiCharacterFlags = value;
            }
        }

        #endregion
    }
}