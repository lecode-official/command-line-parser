
#region Using Directives

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Xunit;

#endregion

namespace System.CommandLine.Tests
{
    /// <summary>
    /// Represents a set of unit tests for the command line parser.
    /// </summary>
    public class ParserTests
    {
        #region Unit Tests

        /// <summary>
        /// Tests whether the command line parser correctly handles situations where multiple arguments are declared with the same name and/or alias.
        /// </summary>
        [Fact]
        public void TestDuplicateArgumentReferences()
        {
            // Adding two arguments with the same name results in an exception
            Parser parser = new Parser(new ParserOptions { IgnoreCase = true });
            parser.AddFlagArgument<bool>("show-warnings");
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<bool>("show-warnings"));
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<bool>("Show-Warnings"));

            // Adding two arguments with the same alias results in an exception
            parser = new Parser(new ParserOptions { IgnoreCase = true });
            parser.AddNamedArgument<string>("file-name", "f");
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<float>("factor", "f"));
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<StringBuilder>("filter", "F"));

            // Adding two arguments with different names that both map to the same destination name results in an exception
            parser = new Parser(new ParserOptions { IgnoreCase = true });
            parser.AddPositionalArgument<string>("file-name");
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<StringBuilder>("fileName"));
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<StringBuilder>("FileName"));

            // When unsetting the ignore case option, then two names that differ in casing should be accepted
            parser = new Parser(new ParserOptions { IgnoreCase = false });
            parser.AddFlagArgument<bool>("show-warnings");
            parser.AddNamedArgument<bool>("Show-Warnings", null, "Warnings", null);
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<bool>("show-warnings"));

            // When unsetting the ignore case option, then two aliases that differ in casing should be accepted
            parser = new Parser(new ParserOptions { IgnoreCase = false });
            parser.AddNamedArgument<string>("file-name", "f");
            parser.AddNamedArgument<float>("factor", "F");
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<StringBuilder>("filter", "f"));

            // When unsetting the ignore case option, then two destinations that differ in casing should be accepted
            parser = new Parser(new ParserOptions { IgnoreCase = false });
            parser.AddNamedArgument<bool>("show-warnings", null, "ShowWarnings", null);
            parser.AddNamedArgument<bool>("Show-Warnings", null, "Showwarnings", null);
            Assert.Throws<InvalidOperationException>(() => parser.AddNamedArgument<bool>("show-warnings"));
        }

        /// <summary>
        /// Tests whether the command line parser correctly handles situations where multiple commands are declared with the same name.
        /// </summary>
        [Fact]
        public void TestDuplicateCommandNames()
        {
            // Adding two commands with the same name results in an exception
            Parser parser = new Parser(new ParserOptions { IgnoreCase = true });
            parser.AddCommand("create");
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("Create"));
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("create"));

            // Adding two commands with the same alias results in an exception
            parser = new Parser(new ParserOptions { IgnoreCase = true });
            parser.AddCommand("create", "c", "Creates a new entity.");
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("combine", "c", "Combines two entities."));
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("calibrate", "C", "Calibrates the entity."));

            // When unsetting the ignore case option, then two names that differ in casing should be accepted
            parser = new Parser(new ParserOptions { IgnoreCase = false });
            parser.AddCommand("create");
            parser.AddCommand("Create");
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("create"));

            // When unsetting the ignore case option, then two aliases that differ in casing should be accepted
            parser = new Parser(new ParserOptions { IgnoreCase = false });
            parser.AddCommand("create", "c", "Creates a new entity.");
            parser.AddCommand("combine", "C", "Combines two entities.");
            Assert.Throws<InvalidOperationException>(() => parser.AddCommand("calibrate", "C", "Calibrates the entity."));
        }

        /// <summary>
        /// Tests whether the parser can handle one positional argument.
        /// </summary>
        [Fact]
        public void TestOnePositionalArgument()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser.AddPositionalArgument<string>("file-name");

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", @"C:\test" });

            // Validates that the parsed values are correct
            Assert.Equal(@"C:\test", parsingResults.GetParsedValue<string>("FileName"));
        }

        /// <summary>
        /// Tests whether the parser can handle multiple positional arguments.
        /// </summary>
        [Fact]
        public void TestMultiplePositionalArgument()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser
                .AddPositionalArgument<string>("file-name")
                .AddPositionalArgument<double>("factor")
                .AddPositionalArgument<DayOfWeek>("day-of-week");

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", @"C:\test", "8.45e9", "friday" });

            // Validates that the parsed values are correct
            Assert.Equal(@"C:\test", parsingResults.GetParsedValue<string>("FileName"));
            Assert.Equal(8.45e9, parsingResults.GetParsedValue<double>("Factor"));
            Assert.Equal(DayOfWeek.Friday, parsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests how the parser handles if a position argument is missing.
        /// </summary>
        [Fact]
        public void TestMissingPositionalArguments()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser
                .AddPositionalArgument<string>("file-name")
                .AddPositionalArgument<double>("factor");

            // Parses the command line arguments, since there are two positional arguments declared and only one value was supplied, an exception should be raised
            Assert.Throws<InvalidOperationException>(() => parser.Parse(new string[] { "test.exe", @"C:\test" }));
        }

        /// <summary>
        /// Tests whether the parser can handle one named argument.
        /// </summary>
        [Fact]
        public void TestOneNamedArgument()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddNamedArgument<string>("file-name", "f");

            // Parses the command line arguments
            ParsingResults parsingResultsWithName = parser.Parse(new string[] { "test.exe", "--file-name", @"C:\test" });
            ParsingResults parsingResultsWithAlias = parser.Parse(new string[] { "test.exe", "-f", @"C:\test" });

            // Validates that the parsed values are correct
            Assert.Equal(@"C:\test", parsingResultsWithName.GetParsedValue<string>("FileName"));
            Assert.Equal(@"C:\test", parsingResultsWithAlias.GetParsedValue<string>("FileName"));
        }

        /// <summary>
        /// Tests whether the parser can handle multiple named arguments.
        /// </summary>
        [Fact]
        public void TestMultipleNamedArgument()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "/",
                ArgumentAliasPrefix = "/"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i")
                .AddNamedArgument<DayOfWeek>("day-of-week", "d");

            // Parses the command line arguments
            ParsingResults parsingResultsWithName = parser.Parse(new string[] { "test.exe", "/feature", "false", "/number-of-iterations", "72", "/day-of-week", "wednesday" });
            ParsingResults parsingResultsWithAlias = parser.Parse(new string[] { "test.exe", "/f", "off", "/i", "72", "/d", "wednesday" });
            ParsingResults parsingResultsMixed = parser.Parse(new string[] { "test.exe", "/f", "no", "/number-of-iterations", "72", "/d", "wednesday" });

            // Validates that the parsed values are correct
            Assert.False(parsingResultsWithName.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResultsWithName.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, parsingResultsWithName.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.False(parsingResultsWithAlias.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResultsWithAlias.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, parsingResultsWithAlias.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.False(parsingResultsMixed.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResultsMixed.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, parsingResultsMixed.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests whether the parser correctly handles default values.
        /// </summary>
        [Fact]
        public void TestNamedArgumentDefaultValues()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser
                .AddNamedArgument<bool>("feature", "f", "Feature", null, true)
                .AddNamedArgument<int>("number-of-iterations", "i", "NumberOfIterations", null, 123)
                .AddNamedArgument<DayOfWeek>("day-of-week", "d", "DayOfWeek");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "--number-of-iterations", "72" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "--feature", "false", "-i", "72", "-d", "wednesday" });

            // Validates that the parsed values are correct
            Assert.True(firstParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(123, firstParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Sunday, firstParsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.True(secondParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, secondParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Sunday, secondParsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.False(thirdParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, thirdParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, thirdParsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests whether the parser can handle one flag argument.
        /// </summary>
        [Fact]
        public void TestOneFlagArgument()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddFlagArgument<int>("number", "n");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "--number" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "--number", "--number" });
            ParsingResults fourthParsingResults = parser.Parse(new string[] { "test.exe", "-n", "-n", "-n" });

            // Validates that the parsed values are correct
            Assert.Equal(0, firstParsingResults.GetParsedValue<int>("Number"));
            Assert.Equal(1, secondParsingResults.GetParsedValue<int>("Number"));
            Assert.Equal(2, thirdParsingResults.GetParsedValue<int>("Number"));
            Assert.Equal(3, fourthParsingResults.GetParsedValue<int>("Number"));
        }

        /// <summary>
        /// Tests whether the parser can handle multiple flag arguments.
        /// </summary>
        [Fact]
        public void TestMultipleFlagArgument()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddFlagArgument<DayOfWeek>("day", "d");
            parser.AddFlagArgument<bool>("verbose", "v");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "--verbose", "--day" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "--day", "--day" });
            ParsingResults fourthParsingResults = parser.Parse(new string[] { "test.exe", "-d", "-v", "-d", "-v", "-d" });

            // Validates that the parsed values are correct
            Assert.Equal(DayOfWeek.Sunday, firstParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.False(firstParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Monday, secondParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.True(secondParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Tuesday, thirdParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.False(thirdParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Wednesday, fourthParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.True(fourthParsingResults.GetParsedValue<bool>("Verbose"));
        }

        /// <summary>
        /// Represents an enumeration used for testing the multi-character flags.
        /// </summary>
        private enum VerbosityLevel { Quiet, Minimal, Normal, Detailed, Diagnostic }

        /// <summary>
        /// Tests how the parser handles multi-character flags.
        /// </summary>
        [Fact]
        public void TestMultiCharacterFlagArguments()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddFlagArgument<VerbosityLevel>("verbosity", "v");
            parser.AddFlagArgument<int>("count", "c");
            parser.AddFlagArgument<bool>("activate", "a");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "-vca" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "-vvv", "-ccc", "-aaa" });
            ParsingResults fourthParsingResults = parser.Parse(new string[] { "test.exe", "-vvvcccaaa" });

            // Validates that the parsed values are correct
            Assert.Equal(VerbosityLevel.Quiet, firstParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(0, firstParsingResults.GetParsedValue<int>("Count"));
            Assert.False(firstParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.Minimal, secondParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(1, secondParsingResults.GetParsedValue<int>("Count"));
            Assert.True(secondParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.Detailed, thirdParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(3, thirdParsingResults.GetParsedValue<int>("Count"));
            Assert.True(thirdParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.Detailed, fourthParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(3, fourthParsingResults.GetParsedValue<int>("Count"));
            Assert.True(fourthParsingResults.GetParsedValue<bool>("Activate"));
        }

        /// <summary>
        /// Tests how the parser handles a situation where it parses a named argument that was not declared.
        /// </summary>
        [Fact]
        public void TestUnknownNamedArgument()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments, since there is are only two named arguments declared but three are supplied, an exception should be raised
            Assert.Throws<InvalidOperationException>(() => parser.Parse(new string[] { "test.exe", "--feature", "on", "--number-of-iterations", "72", "--day-of-week", "wednesday" }));
        }

        /// <summary>
        /// Tests how the parser handles a situation where it parses a named argument and its value is missing.
        /// </summary>
        [Fact]
        public void TestMissingNamedArgumentValue()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "/",
                ArgumentAliasPrefix = "/"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i")
                .AddNamedArgument<DayOfWeek>("day-of-week", "d");

            // Parses the command line arguments, there are three declared arguments and only two are supplied, but since named arguments are optional, nothing should happen
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "/feature", "yes", "/number-of-iterations", "72" });

            // Validates that the parsed values are correct
            Assert.True(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResults.GetParsedValue<int>("NumberOfIterations"));
        }

        /// <summary>
        /// Tests whether the parser can handle multiple positional arguments and named arguments.
        /// </summary>
        [Fact]
        public void TestPositionalAndNamedArguments()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser
                .AddPositionalArgument<string>("file-name")
                .AddPositionalArgument<float>("factor")
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i")
                .AddNamedArgument<DayOfWeek>("day-of-week", "d");

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "/home/test.txt", "123.5", "-f", "true", "--number-of-iterations", "123", "-d", "Sunday" });

            // Validates that the parsed values are correct
            Assert.Equal("/home/test.txt", parsingResults.GetParsedValue<string>("FileName"));
            Assert.Equal(123.5, parsingResults.GetParsedValue<float>("Factor"));
            Assert.True(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(123, parsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Sunday, parsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests how the parser handles different prefix styles (e.g. the UNIX "--" style and the Windows "/" style).
        /// </summary>
        [Fact]
        public void TestArgumentPrefixStyles()
        {
            // Sets up the parser with the usual UNIX style argument prefixes
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with the UNIX style argument prefixes
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "--feature", "off", "-i", "987" });

            // Validates that the parsed values with the UNIX style argument prefixes are correct
            Assert.False(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));

            // Sets up the parser with the usual Windows style argument prefixes
            parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "/",
                ArgumentAliasPrefix = "/"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with the Windows style argument prefixes
            parsingResults = parser.Parse(new string[] { "test.exe", "/feature", "off", "/i", "987" });

            // Validates that the parsed values with the Windows style argument prefixes are correct
            Assert.False(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));

            // Sets up the parser with some non-standard argument prefixes, with different prefixes for arguments and their aliases
            parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "@",
                ArgumentAliasPrefix = "&"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with non-standard argument prefixes
            parsingResults = parser.Parse(new string[] { "test.exe", "@feature", "off", "&i", "987" });

            // Validates that the parsed values with non-standard argument prefixes are correct
            Assert.False(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));

            // Sets up the parser with some non-standard argument prefixes, with the same prefixes for arguments and their aliases
            parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "=",
                ArgumentAliasPrefix = "="
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with non-standard argument prefixes
            parsingResults = parser.Parse(new string[] { "test.exe", "=feature", "off", "=i", "987" });

            // Validates that the parsed values with non-standard argument prefixes are correct
            Assert.False(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));

            // Sets up the parser with switched UNIX style argument prefixes
            parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "-",
                ArgumentAliasPrefix = "--"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with the switched UNIX style argument prefixes
            parsingResults = parser.Parse(new string[] { "test.exe", "-feature", "off", "--i", "987" });

            // Validates that the parsed values with the switched UNIX style argument prefixes are correct
            Assert.False(parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));
        }

        /// <summary>
        /// Tests how the parser handles named arguments that are using the key value separator syntax (e.g. "--age=18" instead of "--age 18").
        /// </summary>
        [Fact]
        public void TestKeyValueSeparator()
        {
            // Sets up the parser
            Parser unixStyleParser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-",
                KeyValueSeparator = "="
            });
            unixStyleParser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");
            Parser windowsStyleParser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "/",
                ArgumentAliasPrefix = "/",
                KeyValueSeparator = ":"
            });
            windowsStyleParser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments
            ParsingResults firstUnixStyleParsingResults = unixStyleParser.Parse(new string[] { "test.exe", "--feature=true", "-i=34" });
            ParsingResults secondUnixStyleParsingResults = unixStyleParser.Parse(new string[] { "test.exe", "-f", "off", "--number-of-iterations", "78" });
            ParsingResults firstWindowStyleParsingResults = windowsStyleParser.Parse(new string[] { "test.exe", "/feature:no", "/i:49" });
            ParsingResults secondWindowsStyleParsingResults = windowsStyleParser.Parse(new string[] { "test.exe", "/f", "yes", "/number-of-iterations", "20" });

            // Validates that the parsed values are correct
            Assert.True(firstUnixStyleParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(34, firstUnixStyleParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.False(secondUnixStyleParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(78, secondUnixStyleParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.False(firstWindowStyleParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(49, firstWindowStyleParsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.True(secondWindowsStyleParsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(20, secondWindowsStyleParsingResults.GetParsedValue<int>("NumberOfIterations"));
        }

        /// <summary>
        /// Tests how the parser handles the parsing of arguments of a collection type and especially how it handles a situation where a single named argument is provided multiple times.
        /// </summary>
        [Fact]
        public void TestCollectionArguments()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddNamedArgument<List<string>>("names", "n");
            parser.AddNamedArgument<string>("car", "c");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "--names", "Alice", "-n", "Bob", "--car", "blue car", "--names", "Eve" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "-n", "Alice", "Bob", "Eve", "-c", "red car" });

            // Validates that the parsed values are correct
            Assert.Equal(new List<string> { "Alice", "Bob", "Eve" }, firstParsingResults.GetParsedValue<List<string>>("Names"));
            Assert.Equal("blue car", firstParsingResults.GetParsedValue<string>("Car"));
            Assert.Equal(new List<string> { "Alice", "Bob", "Eve" }, secondParsingResults.GetParsedValue<List<string>>("Names"));
            Assert.Equal("red car", secondParsingResults.GetParsedValue<string>("Car"));
        }

        /// <summary>
        /// Tests whether the default duplicate resolution policy works, which is used if a single named argument is provided more than once.
        /// </summary>
        [Fact]
        public void TestDuplicateResolutionPolicy()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddNamedArgument<int>("age", "a");

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "--age", "18", "-a", "17", "--age", "16" });

            // Validates that the parsed values are correct
            Assert.Equal(16, parsingResults.GetParsedValue<int>("Age"));
        }

        /// <summary>
        /// Tests whether a custom duplicate resolution policy works, which is used if a single named argument is provided more than once.
        /// </summary>
        [Fact]
        public void TestCustomDuplicateResolutionPolicy()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddNamedArgument<int>("add", "a", "Add", "Adds numbers together", 0, (a, b) => a + b);
            parser.AddNamedArgument<int>("multiply", "m", "Multiply", "Multiplies numbers together", 0, (a, b) => a * b);

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "--add", "1", "-a", "2", "--add", "3" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "--multiply", "4", "5", "-m", "6" });

            // Validates that the parsed values are correct
            Assert.Equal(6, firstParsingResults.GetParsedValue<int>("Add"));
            Assert.Equal(120, secondParsingResults.GetParsedValue<int>("Multiply"));
        }

        /// <summary>
        /// Tests how the parser handles the parsing of a single commands.
        /// </summary>
        [Fact]
        public void TestSingleCommand()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser.AddNamedArgument<bool>("verbose", "v");
            Parser commandParser = parser.AddCommand("restart", "r", string.Empty);
            commandParser.AddPositionalArgument<string>("time");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "-v", "no" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "restart", "now" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "--verbose", "yes", "r", "later" });

            // Validates that the parsed values are correct
            Assert.False(firstParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.True(secondParsingResults.HasSubResults);
            Assert.NotNull(secondParsingResults.SubResults);
            Assert.Equal("restart", secondParsingResults.SubResults.Command);
            Assert.Equal("now", secondParsingResults.SubResults.GetParsedValue<string>("Time"));
            Assert.True(thirdParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.True(thirdParsingResults.HasSubResults);
            Assert.NotNull(thirdParsingResults.SubResults);
            Assert.Equal("restart", thirdParsingResults.SubResults.Command);
            Assert.Equal("later", thirdParsingResults.SubResults.GetParsedValue<string>("Time"));
        }

        /// <summary>
        /// Tests how the parser handles the parsing of a command when having the a choice between multiple commands.
        /// </summary>
        [Fact]
        public void TestMultipleCommands()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser.AddCommand("test", "t", string.Empty);
            parser.AddCommand("run", "r", string.Empty);

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "test" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "t" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "run" });
            ParsingResults fourthParsingResults = parser.Parse(new string[] { "test.exe", "r" });

            // Validates that the parsed values are correct
            Assert.Equal("test", firstParsingResults.SubResults.Command);
            Assert.Equal("test", secondParsingResults.SubResults.Command);
            Assert.Equal("run", thirdParsingResults.SubResults.Command);
            Assert.Equal("run", fourthParsingResults.SubResults.Command);
        }

        /// <summary>
        /// Tests how the parser handles the parsing of multiple nested commands.
        /// </summary>
        [Fact]
        public void TestNestedCommands()
        {
            // Sets up the parser
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            Parser subParser = parser.AddCommand("add");
            Parser subSubParser = subParser.AddCommand("package");
            subSubParser.AddCommand("to-project");
            subSubParser.AddCommand("to-solution");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "add" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "add", "package" });
            ParsingResults fourthParsingResults = parser.Parse(new string[] { "test.exe", "add", "package", "to-project" });
            ParsingResults fifthParsingResults = parser.Parse(new string[] { "test.exe", "add", "package", "to-solution" });

            // Validates that the parsed values are correct
            Assert.False(firstParsingResults.HasSubResults);
            Assert.Equal("add", secondParsingResults.SubResults.Command);
            Assert.False(secondParsingResults.SubResults.HasSubResults);
            Assert.Equal("add", thirdParsingResults.SubResults.Command);
            Assert.Equal("package", thirdParsingResults.SubResults.SubResults.Command);
            Assert.False(thirdParsingResults.SubResults.SubResults.HasSubResults);
            Assert.Equal("add", fourthParsingResults.SubResults.Command);
            Assert.Equal("package", fourthParsingResults.SubResults.SubResults.Command);
            Assert.Equal("to-project", fourthParsingResults.SubResults.SubResults.SubResults.Command);
            Assert.False(fourthParsingResults.SubResults.SubResults.SubResults.HasSubResults);
            Assert.Equal("add", fifthParsingResults.SubResults.Command);
            Assert.Equal("package", fifthParsingResults.SubResults.SubResults.Command);
            Assert.Equal("to-solution", fifthParsingResults.SubResults.SubResults.SubResults.Command);
            Assert.False(fifthParsingResults.SubResults.SubResults.SubResults.HasSubResults);
        }

        /// <summary>
        /// Tests how the parser handles situations where arguments have the same names or aliases as commands.
        /// </summary>
        [Fact]
        public void TestArgumentCommandNameCollision()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser.AddNamedArgument<string>("test", "t");
            parser.AddCommand("test", "t", string.Empty);

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "--test", "test", "test" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "-t", "t", "t" });

            // Validates that the parsed values are correct
            Assert.Equal("test", firstParsingResults.GetParsedValue<string>("Test"));
            Assert.True(firstParsingResults.HasSubResults);
            Assert.Equal("test", firstParsingResults.SubResults.Command);
            Assert.Equal("t", secondParsingResults.GetParsedValue<string>("Test"));
            Assert.True(secondParsingResults.HasSubResults);
            Assert.Equal("test", secondParsingResults.SubResults.Command);
        }

        /// <summary>
        /// Tests how the parser handles name collisions in a parser and a sub parser.
        /// </summary>
        [Fact]
        public void TestArgumentNameCollisionInCommands()
        {
            // Sets up the parser
            Parser parser = new Parser();
            parser.AddNamedArgument<DayOfWeek>("day-of-week", "d");
            Parser subParser = parser.AddCommand("test", "t", string.Empty);
            subParser.AddNamedArgument<DayOfWeek>("day-of-week", "d");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe", "--day-of-week", "thursday", "test", "--day-of-week", "wednesday" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "-d", "Tuesday", "t", "-d", "Saturday" });

            // Validates that the parsed values are correct
            Assert.Equal(DayOfWeek.Thursday, firstParsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.True(firstParsingResults.HasSubResults);
            Assert.Equal("test", firstParsingResults.SubResults.Command);
            Assert.Equal(DayOfWeek.Wednesday, firstParsingResults.SubResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.Equal(DayOfWeek.Tuesday, secondParsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.True(secondParsingResults.HasSubResults);
            Assert.Equal("test", secondParsingResults.SubResults.Command);
            Assert.Equal(DayOfWeek.Saturday, secondParsingResults.SubResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests how the parser handles a very complex scenario with many different kinds of parameters and multiple commands.
        /// </summary>
        [Fact]
        public void TestComplexScenario()
        {
            // Creates the main parser and adds all the necessary arguments
            Parser parser = new Parser("A simple version of the .NET CLI.", new ParserOptions(false, "--", "-", "=", true));
            parser.AddFlagArgument<bool>("help", "h", "Show command line help.");

            // Creates the parser for the add command
            Parser addCommandParser = parser.AddCommand("add", "Add a new package or reference to a .NET project.");
            addCommandParser.AddPositionalArgument<string>("project", "The project file to operate on.");
            addCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");

            // Creates the parser for the add package command
            Parser addPackageCommandParser = addCommandParser.AddCommand("package", "Add a NuGet package reference to the project.");
            addPackageCommandParser.AddPositionalArgument<string>("package-name", "The package reference to add.");
            addPackageCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");
            addPackageCommandParser.AddNamedArgument<string>("version", "v", "The version of the package to add.");
            addPackageCommandParser.AddFlagArgument<bool>("interactive", null, "Show command line help.");

            // Creates the parser for the add reference command
            Parser addReferenceCommandParser = addCommandParser.AddCommand("reference", "Add a project-to-project reference to the project.");
            addReferenceCommandParser.AddPositionalArgument<string>("project-path", "The paths to the projects to add as references.");
            addReferenceCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");
            addReferenceCommandParser.AddNamedArgument<string>("framework", "f", "Add the reference only when targeting a specific framework.");

            // Creates the parser for the build command
            Parser buildCommandParser = parser.AddCommand("build", "Add a new package or reference to a .NET project.");
            buildCommandParser.AddPositionalArgument<string>("project", "The project file to operate on.");
            buildCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");
            buildCommandParser.AddNamedArgument<VerbosityLevel>("verbosity", "v", "Set the MSBuild verbosity level. Allowed values are quiet, minimal, normal, detailed, and diagnostic.");
            buildCommandParser.AddNamedArgument<string>("framework", "f", "The target framework to build for. The target framework must also be specified in the project file.");

            // Creates the parser for the new command
            Parser newCommandParser = parser.AddCommand("new", "Create a new .NET project or file.");
            buildCommandParser.AddPositionalArgument<string>("template", "The project template to use.");
            newCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");
            buildCommandParser.AddNamedArgument<string>("name", "n", "The name for the output being created. If no name is specified, the name of the current directory is used.");
            buildCommandParser.AddNamedArgument<string>("output", "o", "Location to place the generated output.");

            // Creates the parser for the run command
            Parser runCommandParser = parser.AddCommand("run", "Build and run a .NET project output.");
            runCommandParser.AddFlagArgument<bool>("help", "h", "Show command line help.");
            runCommandParser.AddNamedArgument<string>("project", "p", "Project", "The path to the project file to run (defaults to the current directory if there is only one project).", "./");
            runCommandParser.AddNamedArgument<string>("framework", "f", "The target framework to run for. The target framework must also be specified in the project file.");

            // Tests how the parser handles no command line arguments at all
            ParsingResults parsingResults = parser.Parse(new string[] { "dotnet" });
            Assert.False(parsingResults.GetParsedValue<bool>("Help"));
            Assert.False(parsingResults.HasSubResults);
            Assert.Null(parsingResults.SubResults);

            // Tests how the parser handles the add command
            parsingResults = parser.Parse(new string[] { "dotnet", "add", "./test.csproj", "--help" });
            Assert.False(parsingResults.GetParsedValue<bool>("Help"));
            Assert.True(parsingResults.HasSubResults);
            Assert.Equal("add", parsingResults.SubResults.Command);
            Assert.Equal("./test.csproj", parsingResults.SubResults.GetParsedValue<string>("Project"));
            Assert.True(parsingResults.SubResults.GetParsedValue<bool>("Help"));

            // Tests how the parser handles the missing package name in the add package command
            Assert.Throws<InvalidOperationException>(() => parser.Parse(new string[] { "dotnet", "add", "./test.csproj", "package" }));
        }

        #endregion
    }
}
