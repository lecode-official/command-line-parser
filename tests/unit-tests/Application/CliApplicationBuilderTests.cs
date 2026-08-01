
#region Using Directives

using CliNetCore.Application;
using CliNetCore.UnitTests.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Application;

/// <summary>
/// Tests <see cref="CliApplicationBuilder"/>, verifying that command-line arguments and services registered on the builder flow through to the
/// built <see cref="CliApplication"/>, that <see cref="CliCommandDispatcherService"/> is wired up as a hosted service, and that the
/// <see cref="IHostApplicationBuilder"/> surface it exposes forwards to a real, usable builder. Builders are created through
/// <see cref="CliApplicationTestBase"/> rather than <see cref="CliApplicationBuilder.CreateBuilder()"/> directly, so that a future test in this class
/// that runs the built application reports its logging to this test's <see cref="ITestOutputHelper"/> instead of the console.
/// </summary>
public sealed class CliApplicationBuilderTests : CliApplicationTestBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplicationBuilderTests"/> class.
    /// </summary>
    /// <param name="output">The output helper xUnit injects for the currently running test.</param>
    public CliApplicationBuilderTests(ITestOutputHelper output)
        : base(output)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Verifies that <see cref="CliApplicationBuilder.CreateBuilder()"/> registers an empty <see cref="CliCommandLineArguments"/>, rather than
    /// leaving it unregistered, when no arguments are given.
    /// </summary>
    [Fact]
    public void CreateBuilderGivenNoArgumentsRegistersEmptyCliCommandLineArguments()
    {
        CliApplicationBuilder builder = this.CreateBuilder();

        using CliApplication application = builder.Build();

        Assert.Empty(application.Services.GetRequiredService<CliCommandLineArguments>().Arguments);
    }

    /// <summary>
    /// Verifies that <see cref="CliApplicationBuilder.CreateBuilder(string[])"/> registers the given arguments as a <see cref="CliCommandLineArguments"/>
    /// resolvable from the built application's <see cref="IServiceProvider"/>.
    /// </summary>
    [Fact]
    public void CreateBuilderGivenArgumentsRegistersThemAsCliCommandLineArguments()
    {
        string[] expectedArguments = ["greet", "--name", "David"];
        CliApplicationBuilder builder = this.CreateBuilder(expectedArguments);

        using CliApplication application = builder.Build();

        Assert.Equal(expectedArguments, application.Services.GetRequiredService<CliCommandLineArguments>().Arguments);
    }

    /// <summary>
    /// Verifies that a service registered on <see cref="CliApplicationBuilder.Services"/> before <see cref="CliApplicationBuilder.Build"/> is called
    /// is resolvable from the built <see cref="CliApplication"/>, confirming that <see cref="CliApplication.Services"/> is backed by the same
    /// service collection the consumer configured, not a separate one.
    /// </summary>
    [Fact]
    public void BuildServicesRegisteredOnTheBuilderAreResolvableFromTheBuiltApplication()
    {
        CliApplicationBuilder builder = this.CreateBuilder();
        builder.Services.AddSingleton("configured-through-the-builder");

        using CliApplication application = builder.Build();

        Assert.Equal("configured-through-the-builder", application.Services.GetRequiredService<string>());
    }

    /// <summary>
    /// Verifies that <see cref="CliApplicationBuilder.Build"/> registers <see cref="CliCommandDispatcherService"/> as an <see cref="IHostedService"/>,
    /// which is what makes the built application run a command and shut down instead of idling forever like a typical long-running host.
    /// </summary>
    [Fact]
    public void BuildRegistersTheCliCommandDispatcherServiceAsAHostedService()
    {
        CliApplicationBuilder builder = this.CreateBuilder();

        using CliApplication application = builder.Build();

        Assert.Contains(application.Services.GetServices<IHostedService>(), hostedService => hostedService is CliCommandDispatcherService);
    }

    /// <summary>
    /// Verifies that every member <see cref="CliApplicationBuilder"/> exposes to satisfy <see cref="IHostApplicationBuilder"/> forwards to a real,
    /// non-<see langword="null"/> object on the underlying builder, rather than one of them accidentally being left unimplemented.
    /// </summary>
    [Fact]
    public void HostApplicationBuilderMembersAllNonNull()
    {
        CliApplicationBuilder builder = this.CreateBuilder();

        Assert.NotNull(builder.Properties);
        Assert.NotNull(builder.Configuration);
        Assert.NotNull(builder.Environment);
        Assert.NotNull(builder.Logging);
        Assert.NotNull(builder.Metrics);
        Assert.NotNull(builder.Services);
    }

    #endregion
}
