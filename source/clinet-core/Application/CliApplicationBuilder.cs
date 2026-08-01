
#region Using Directives

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace CliNetCore.Application;

/// <summary>
/// A builder for configuring and creating a <see cref="CliApplication"/>. It is the CLI.NET Core analogue of ASP.NET Core's
/// <c>WebApplicationBuilder</c>: It implements <see cref="IHostApplicationBuilder"/> so that the whole generic-host ecosystem (hosted-service
/// registration, configuration, logging, dependency injection, and tooling such as <c>dotnet run</c>) works against it unchanged, while internally it
/// merely wraps a concrete <see cref="HostApplicationBuilder"/> and forwards to it. The concrete builder is <see langword="sealed"/> and therefore
/// cannot be sub-classed, so composition, not inheritance, is used, which also lets this type expose a small, CLI-shaped surface instead of the full
/// host builder surface.
/// </summary>
public sealed class CliApplicationBuilder : IHostApplicationBuilder
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplicationBuilder"/> class. The constructor is <see langword="internal"/>, because
    /// <see cref="CliApplicationBuilder"/> instances can only be created through the <see cref="CreateBuilder()"/> or
    /// <see cref="CreateBuilder(string[])"/> factory methods, or, even better, through the <see cref="CliApplication.CreateBuilder()"/> or
    /// <see cref="CliApplication.CreateBuilder(string[])"/> factory methods.
    /// </summary>
    /// <param name="settings">
    /// The settings used to initialize the underlying <see cref="HostApplicationBuilder"/>, most notably the command-line arguments. May be
    /// <see langword="null"/>, in which case the host defaults are used.
    /// </param>
    internal CliApplicationBuilder(HostApplicationBuilderSettings? settings)
    {
        this.hostApplicationBuilder = Host.CreateApplicationBuilder(settings);

        // Registered unconditionally, defaulting to an empty list, so that command logic can always depend on CliCommandLineArguments instead of
        // reading Environment.GetCommandLineArgs() directly, regardless of whether the application was created with arguments
        this.hostApplicationBuilder.Services.AddSingleton(new CliCommandLineArguments(settings?.Args ?? []));
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The concrete generic-host builder that does the actual work. Every member of this wrapper delegates to it, and <see cref="Build"/> ultimately
    /// calls its <see cref="HostApplicationBuilder.Build"/> to produce the real <see cref="IHost"/> that the returned <see cref="CliApplication"/>
    /// wraps.
    /// </summary>
    private readonly HostApplicationBuilder hostApplicationBuilder;

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> pre-configured with the host defaults and no command-line arguments.
    /// </summary>
    /// <returns>Returns the newly created builder.</returns>
    public static CliApplicationBuilder CreateBuilder() => new(new HostApplicationBuilderSettings());

    /// <summary>
    /// Creates a <see cref="CliApplicationBuilder"/> pre-configured with the host defaults and the given command-line arguments. This is the usual
    /// entry point for a CLI application; the <paramref name="args"/> are the same array that is passed to the program's <c>Main</c> method.
    /// </summary>
    /// <param name="args">The command-line arguments the application was started with.</param>
    /// <returns>Returns the newly created builder.</returns>
    public static CliApplicationBuilder CreateBuilder(string[] args) => new(new HostApplicationBuilderSettings { Args = args });

    #endregion

    #region Public Methods

    /// <summary>
    /// Builds the <see cref="CliApplication"/>. This registers the infrastructure the CLI needs, builds the underlying generic host, and wraps it in
    /// a <see cref="CliApplication"/>. After this method returns, the underlying host is frozen: its service provider has been built and no further
    /// configuration is possible, which mirrors the "frozen after <c>Build()</c>" invariant of the generic host.
    /// </summary>
    /// <returns>Returns the built <see cref="CliApplication"/>, ready to be run.</returns>
    public CliApplication Build()
    {
        // The dispatcher hosted service is registered last (during Build, after all of the consumer's ConfigureServices calls have run) so that it
        // starts after — and therefore stops before — any hosted services the consumer registered themselves.
        this.hostApplicationBuilder.Services.AddHostedService<CliCommandDispatcherService>();

        return new CliApplication(this.hostApplicationBuilder.Build());
    }

    #endregion

    #region IHostApplicationBuilder Implementation

    /// <inheritdoc/>
    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)this.hostApplicationBuilder).Properties;

    /// <inheritdoc/>
    public IConfigurationManager Configuration => this.hostApplicationBuilder.Configuration;

    /// <inheritdoc/>
    public IHostEnvironment Environment => this.hostApplicationBuilder.Environment;

    /// <inheritdoc/>
    public ILoggingBuilder Logging => this.hostApplicationBuilder.Logging;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => this.hostApplicationBuilder.Metrics;

    /// <inheritdoc/>
    public IServiceCollection Services => this.hostApplicationBuilder.Services;

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null)
        where TContainerBuilder : notnull =>
        this.hostApplicationBuilder.ConfigureContainer(factory, configure);

    #endregion
}
