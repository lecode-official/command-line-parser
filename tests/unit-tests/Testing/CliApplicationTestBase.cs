
#region Using Directives

using CliNetCore.Application;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Testing;

/// <summary>
/// A base class for test classes that build a <see cref="CliApplicationBuilder"/>. xUnit constructs a fresh instance of the test class, and injects a
/// fresh <see cref="ITestOutputHelper"/> through the constructor, once per test method, so deriving from this class and creating every
/// <see cref="CliApplicationBuilder"/> through <see cref="CreateBuilder()"/> or <see cref="CreateBuilder(string[])"/>, instead of
/// <see cref="CliApplicationBuilder.CreateBuilder()"/> or <see cref="CliApplicationBuilder.CreateBuilder(string[])"/> directly, is enough to route
/// every log message the built application writes to that test's own <see cref="ITestOutputHelper"/> instead of to the console, where it would print
/// unconditionally and clutter the <c>dotnet test</c> output of every run, whether or not the test failed. This class is deliberately left
/// unsealed, unlike most other classes in this codebase, because being extended is its entire purpose.
/// </summary>
public abstract class CliApplicationTestBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplicationTestBase"/> class.
    /// </summary>
    /// <param name="output">The output helper xUnit injects for the currently running test, used to redirect the built application's logging.</param>
    protected CliApplicationTestBase(ITestOutputHelper output) => this.output = output;

    #endregion

    #region Private Fields

    /// <summary>
    /// The output helper for the currently running test, captured so that <see cref="CreateBuilder()"/> and <see cref="CreateBuilder(string[])"/> can
    /// register an <see cref="XunitLoggerProvider"/> over it without the deriving test class having to do so itself.
    /// </summary>
    private readonly ITestOutputHelper output;

    #endregion

    #region Protected Methods

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> exactly like <see cref="CliApplicationBuilder.CreateBuilder()"/>, except with its logging
    /// redirected to this test's <see cref="ITestOutputHelper"/>.
    /// </summary>
    /// <returns>Returns the newly created builder.</returns>
    protected CliApplicationBuilder CreateBuilder() => this.RedirectLogging(CliApplicationBuilder.CreateBuilder());

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> exactly like <see cref="CliApplicationBuilder.CreateBuilder(string[])"/>, except with its
    /// logging redirected to this test's <see cref="ITestOutputHelper"/>.
    /// </summary>
    /// <param name="args">The command-line arguments the built application should be created with.</param>
    /// <returns>Returns the newly created builder.</returns>
    protected CliApplicationBuilder CreateBuilder(string[] args) => this.RedirectLogging(CliApplicationBuilder.CreateBuilder(args));

    #endregion

    #region Private Methods

    /// <summary>
    /// Replaces the default logging providers a newly created <paramref name="builder"/> already has with a single <see cref="XunitLoggerProvider"/>
    /// over this test's <see cref="ITestOutputHelper"/>.
    /// </summary>
    /// <param name="builder">The builder whose logging providers should be redirected.</param>
    /// <returns>Returns the same <paramref name="builder"/>, so that this method doubles as a single-expression pass-through.</returns>
    private CliApplicationBuilder RedirectLogging(CliApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new XunitLoggerProvider(this.output));
        return builder;
    }

    #endregion
}
