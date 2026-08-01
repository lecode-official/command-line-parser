
#region Using Directives

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace CliNetCore.Application;

/// <summary>
/// A built CLI application, ready to be run. It is the CLI.NET Core analogue of ASP.NET Core's <c>WebApplication</c>: It implements
/// <see cref="IHost"/> so that it interoperates with the whole generic-host ecosystem, but it does not reimplement hosting and it does not sub-class
/// the concrete host (which is <see langword="internal"/> and <see langword="sealed"/>). Instead it composes a real <see cref="IHost"/> and forwards
/// every <see cref="IHost"/> member to it. The convenience properties are not additional state, they are simply resolved from the one real
/// <see cref="IServiceProvider"/> the inner host owns.
/// </summary>
public sealed class CliApplication : IHost, IAsyncDisposable
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplication"/> class. This constructor is <see langword="internal"/> because
    /// <see cref="CliApplication"/> instances can only be created through the <see cref="CliApplicationBuilder"/> class.
    /// </summary>
    /// <param name="host">The built generic host to wrap. This is produced by <see cref="CliApplicationBuilder.Build"/>.</param>
    internal CliApplication(IHost host) => this.host = host;

    #endregion

    #region Private Fields

    /// <summary>
    /// The concrete generic host that this application wraps. All hosting behavior, i.e., starting and stopping hosted services, owning the service
    /// provider, and coordinating shutdown, is delegated to it.
    /// </summary>
    private readonly IHost host;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the configuration of the application, resolved from the application's service provider.
    /// </summary>
    public IConfiguration Configuration => this.host.Services.GetRequiredService<IConfiguration>();

    /// <summary>
    /// Gets the hosting environment of the application (its name, application name, and content-root path), resolved from the service provider.
    /// </summary>
    public IHostEnvironment Environment => this.host.Services.GetRequiredService<IHostEnvironment>();

    /// <summary>
    /// Gets the application lifetime, which signals application start and shutdown and can be used to request that the application stop.
    /// </summary>
    public IHostApplicationLifetime Lifetime => this.host.Services.GetRequiredService<IHostApplicationLifetime>();

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> with the host defaults and no command-line arguments.
    /// </summary>
    /// <returns>Returns the newly created builder.</returns>
    public static CliApplicationBuilder CreateBuilder() => CliApplicationBuilder.CreateBuilder();

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> with the host defaults and the given command-line arguments. This is the usual entry point for a
    /// CLI application. The <paramref name="args"/> are the same array that is passed to the program's <c>Main</c> method.
    /// </summary>
    /// <param name="args">The command-line arguments the application was started with.</param>
    /// <returns>Returns the newly created builder.</returns>
    public static CliApplicationBuilder CreateBuilder(string[] args) => CliApplicationBuilder.CreateBuilder(args);

    /// <summary>
    /// Runs the application and blocks the calling thread until the host shuts down. For a CLI application this typically means until the command has
    /// finished executing, because the dispatcher hosted service requests shutdown as soon as the command completes. This delegates to the standard
    /// hosting run helper, so it behaves exactly like running any other <see cref="IHost"/>.
    /// </summary>
    public void Run() => this.host.Run();

    /// <summary>
    /// Runs the application and returns a task that completes when the host shuts down. For a CLI application this typically means when the command
    /// has finished executing. This delegates to the standard hosting run helper, so it behaves exactly like running any other <see cref="IHost"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that, when signaled, requests that the application shut down.</param>
    /// <returns>Returns a task that completes when the application has shut down.</returns>
    public Task RunAsync(CancellationToken cancellationToken = default) => this.host.RunAsync(cancellationToken);

    #endregion

    #region IHost Implementation

    /// <inheritdoc/>
    public IServiceProvider Services => this.host.Services;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken = default) => this.host.StartAsync(cancellationToken);

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken = default) => this.host.StopAsync(cancellationToken);

    #endregion

    #region IAsyncDisposable Implementation

    /// <inheritdoc/>
    public void Dispose() => this.host.Dispose();

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        if (this.host is IAsyncDisposable asyncDisposableHost)
            return asyncDisposableHost.DisposeAsync();
        this.host.Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion
}
