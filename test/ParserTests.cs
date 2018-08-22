
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
            Assert.Equal(false, parsingResultsWithName.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResultsWithName.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, parsingResultsWithName.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.Equal(false, parsingResultsWithAlias.GetParsedValue<bool>("Feature"));
            Assert.Equal(72, parsingResultsWithAlias.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Wednesday, parsingResultsWithAlias.GetParsedValue<DayOfWeek>("DayOfWeek"));
            Assert.Equal(false, parsingResultsMixed.GetParsedValue<bool>("Feature"));
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
            Assert.Equal(false, firstParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Monday, secondParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.Equal(true, secondParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Tuesday, thirdParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.Equal(false, thirdParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.Equal(DayOfWeek.Wednesday, fourthParsingResults.GetParsedValue<DayOfWeek>("Day"));
            Assert.Equal(true, fourthParsingResults.GetParsedValue<bool>("Verbose"));
        }

        /// <summary>
        /// Represents an enumeration used for testing the multi-character flags.
        /// </summary>
        private enum VerbosityLevel { None, Low, Medium, High }

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
            Assert.Equal(VerbosityLevel.None, firstParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(0, firstParsingResults.GetParsedValue<int>("Count"));
            Assert.False(firstParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.Low, secondParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(1, secondParsingResults.GetParsedValue<int>("Count"));
            Assert.True(secondParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.High, thirdParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
            Assert.Equal(3, thirdParsingResults.GetParsedValue<int>("Count"));
            Assert.True(thirdParsingResults.GetParsedValue<bool>("Activate"));
            Assert.Equal(VerbosityLevel.High, fourthParsingResults.GetParsedValue<VerbosityLevel>("Verbosity"));
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
            Assert.Equal(true, parsingResults.GetParsedValue<bool>("Feature"));
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
            Assert.Equal(true, parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(123, parsingResults.GetParsedValue<int>("NumberOfIterations"));
            Assert.Equal(DayOfWeek.Sunday, parsingResults.GetParsedValue<DayOfWeek>("DayOfWeek"));
        }

        /// <summary>
        /// Tests how the parser handles different prefix styles (e.g. the Unix "--" style and the Windows "/" style).
        /// </summary>
        [Fact]
        public void TestArgumentPrefixStyles()
        {
            // Sets up the parser with the usual Unix style argument prefixes
            Parser parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "--",
                ArgumentAliasPrefix = "-"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with the Unix style argument prefixes
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "--feature", "off", "-i", "987" });

            // Validates that the parsed values with the Unix style argument prefixes are correct
            Assert.Equal(false, parsingResults.GetParsedValue<bool>("Feature"));
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
            Assert.Equal(false, parsingResults.GetParsedValue<bool>("Feature"));
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
            Assert.Equal(false, parsingResults.GetParsedValue<bool>("Feature"));
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
            Assert.Equal(false, parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));

            // Sets up the parser with switched Unix style argument prefixes
            parser = new Parser(new ParserOptions
            {
                ArgumentPrefix = "-",
                ArgumentAliasPrefix = "--"
            });
            parser
                .AddNamedArgument<bool>("feature", "f")
                .AddNamedArgument<int>("number-of-iterations", "i");

            // Parses the command line arguments with the switched Unix style argument prefixes
            parsingResults = parser.Parse(new string[] { "test.exe", "-feature", "off", "--i", "987" });

            // Validates that the parsed values with the switched Unix style argument prefixes are correct
            Assert.Equal(false, parsingResults.GetParsedValue<bool>("Feature"));
            Assert.Equal(987, parsingResults.GetParsedValue<int>("NumberOfIterations"));
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

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "--names", "Alice", "-n", "Bob", "--names", "Eve" });

            // Validates that the parsed values are correct
            Assert.Equal(new List<string> { "Alice", "Bob", "Eve" }, parsingResults.GetParsedValue<List<string>>("Names"));
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

            // Parses the command line arguments
            ParsingResults parsingResults = parser.Parse(new string[] { "test.exe", "--add", "1", "-a", "2", "--add", "3" });

            // Validates that the parsed values are correct
            Assert.Equal(6, parsingResults.GetParsedValue<int>("Add"));
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
            Assert.Equal(false, firstParsingResults.GetParsedValue<bool>("Verbose"));
            Assert.True(secondParsingResults.HasSubResults);
            Assert.NotNull(secondParsingResults.SubResults);
            Assert.Equal("restart", secondParsingResults.SubResults.Command);
            Assert.Equal("now", secondParsingResults.SubResults.GetParsedValue<string>("Time"));
            Assert.Equal(true, thirdParsingResults.GetParsedValue<bool>("Verbose"));
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
            subParser.AddCommand("package");

            // Parses the command line arguments
            ParsingResults firstParsingResults = parser.Parse(new string[] { "test.exe" });
            ParsingResults secondParsingResults = parser.Parse(new string[] { "test.exe", "add" });
            ParsingResults thirdParsingResults = parser.Parse(new string[] { "test.exe", "add", "package" });

            // Validates that the parsed values are correct
            Assert.False(firstParsingResults.HasSubResults);
            Assert.Equal("add", secondParsingResults.SubResults.Command);
            Assert.False(secondParsingResults.SubResults.HasSubResults);
            Assert.Equal("add", thirdParsingResults.SubResults.Command);
            Assert.Equal("package", thirdParsingResults.SubResults.SubResults.Command);
            Assert.False(thirdParsingResults.SubResults.SubResults.HasSubResults);
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

        #endregion
    }
}
