
#region Using Directives

using CliNetCore.Application;

#endregion

namespace CliNetCore.UnitTests.Application;

/// <summary>
/// Tests <see cref="CliCommandLineArguments"/>, verifying that it exposes exactly the arguments it was constructed with, in their original order,
/// and that an application created without arguments still gets an empty, rather than <see langword="null"/>, instance.
/// </summary>
public sealed class CliCommandLineArgumentsTests
{
    #region Public Methods

    /// <summary>
    /// Verifies that <see cref="CliCommandLineArguments.Arguments"/> returns exactly the arguments passed to the constructor, in their original
    /// order.
    /// </summary>
    [Fact]
    public void ArgumentsGivenArgumentsReturnsThemInOrder()
    {
        string[] expectedArguments = ["build", "--configuration", "Release"];

        CliCommandLineArguments arguments = new(expectedArguments);

        Assert.Equal(expectedArguments, arguments.Arguments);
    }

    /// <summary>
    /// Verifies that <see cref="CliCommandLineArguments.Arguments"/> is empty, rather than <see langword="null"/>, when the instance is constructed
    /// with no arguments.
    /// </summary>
    [Fact]
    public void ArgumentsGivenNoArgumentsIsEmpty()
    {
        CliCommandLineArguments arguments = new([]);

        Assert.Empty(arguments.Arguments);
    }

    #endregion
}
