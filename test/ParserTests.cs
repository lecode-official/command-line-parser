
#region Using Directives

using System;
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

        #endregion
    }
}
