
#region Using Directives

using CliNetCore.Application;
using CliNetCore.UnitTests.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Application;

/// <summary>
/// Tests <see cref="CliApplication"/>, verifying that its static creation methods delegate correctly, that its convenience properties resolve from
/// the same service provider as <see cref="CliApplication.Services"/>, that running the application with no command registered completes and
/// reports a successful exit code, and that both disposal paths forward to the underlying host. Builders are created through
/// <see cref="CliApplicationTestBase"/> rather than <see cref="CliApplication.CreateBuilder()"/> directly, so that
/// <see cref="RunAsyncNoCommandRegisteredCompletesAndReportsSuccess"/>, which actually runs the dispatcher hosted service, reports its log message to
/// this test's <see cref="ITestOutputHelper"/> instead of printing it to the console.
/// </summary>
public sealed class CliApplicationTests : CliApplicationTestBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplicationTests"/> class.
    /// </summary>
    /// <param name="output">The output helper xUnit injects for the currently running test.</param>
    public CliApplicationTests(ITestOutputHelper output)
        : base(output)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Verifies that <see cref="CliApplication.CreateBuilder(string[])"/> delegates to <see cref="CliApplicationBuilder.CreateBuilder(string[])"/>,
    /// so that the arguments given to it end up registered on the built application the same way they would if the builder had been created
    /// directly.
    /// </summary>
    [Fact]
    public void CreateBuilderGivenArgumentsFlowsThroughToTheBuiltApplication()
    {
        string[] expectedArguments = ["greet", "--name", "David"];
        CliApplicationBuilder builder = this.CreateBuilder(expectedArguments);

        using CliApplication application = builder.Build();

        Assert.Equal(expectedArguments, application.Services.GetRequiredService<CliCommandLineArguments>().Arguments);
    }

    /// <summary>
    /// Verifies that <see cref="CliApplication.Configuration"/>, <see cref="CliApplication.Environment"/>, and <see cref="CliApplication.Lifetime"/>
    /// each resolve the same instance <see cref="CliApplication.Services"/> would resolve directly, confirming that they are not separate state but
    /// merely convenience accessors over the one real <see cref="IServiceProvider"/> the application owns.
    /// </summary>
    [Fact]
    public void PropertiesResolveFromTheUnderlyingServiceProvider()
    {
        using CliApplication application = this.CreateBuilder().Build();

        Assert.Same(application.Services.GetRequiredService<IConfiguration>(), application.Configuration);
        Assert.Same(application.Services.GetRequiredService<IHostEnvironment>(), application.Environment);
        Assert.Same(application.Services.GetRequiredService<IHostApplicationLifetime>(), application.Lifetime);
    }

    /// <summary>
    /// Verifies that <see cref="CliApplication.RunAsync"/> completes on its own, without external cancellation, when no command has been registered,
    /// and that it reports <see cref="ExitCode.Success"/> — the placeholder dispatcher behavior documented on
    /// <see cref="CliNetCore.Application.CliCommandDispatcherService"/>.
    /// </summary>
    /// <returns>Returns a task that represents the asynchronous test.</returns>
    [Fact]
    public async Task RunAsyncNoCommandRegisteredCompletesAndReportsSuccess()
    {
        // Environment.ExitCode is process-global state that the dispatcher hosted service writes to, so it is saved and restored around the test
        // instead of being asserted against as if it were owned by this test alone
        int originalExitCode = Environment.ExitCode;
        try
        {
            using CliApplication application = this.CreateBuilder().Build();

            await application.RunAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            Assert.Equal(ExitCode.Success, Environment.ExitCode);
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
    }

    /// <summary>
    /// Verifies that <see cref="CliApplication.Dispose"/> forwards to the underlying host without throwing.
    /// </summary>
    [Fact]
    public void DisposeDisposesTheUnderlyingHost()
    {
        CliApplication application = this.CreateBuilder().Build();

        application.Dispose();
    }

    /// <summary>
    /// Verifies that <see cref="CliApplication.DisposeAsync"/> forwards to the underlying host without throwing.
    /// </summary>
    /// <returns>Returns a task that represents the asynchronous test.</returns>
    [Fact]
    public async Task DisposeAsyncDisposesTheUnderlyingHost()
    {
        CliApplication application = this.CreateBuilder().Build();

        await application.DisposeAsync();
    }

    #endregion
}
