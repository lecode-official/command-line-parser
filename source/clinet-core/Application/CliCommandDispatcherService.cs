
#region Using Directives

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace CliNetCore.Application;

/// <summary>
/// The hosted service that turns a normally long-running generic host into a run-once-and-exit CLI application. Unlike a web server, which stays
/// alive waiting for requests, a CLI tool does its work and then has to terminate on its own. This service does exactly that: Once the host has
/// started, it runs the command corresponding to the command-line arguments a single time, records its exit code in
/// <see cref="Environment.ExitCode"/>, and finally signals <see cref="IHostApplicationLifetime.StopApplication"/> so that the host shuts down and the
/// process returns to the operating system. It derives from <see cref="BackgroundService"/> so that its work runs after the host has finished
/// starting (rather than blocking startup), and the <see cref="CliApplicationBuilder"/> registers it last so that it starts after, and therefore
/// stops before, any hosted services the consumer registered themselves.
/// </summary>
/// <remarks>
/// The body of <see cref="ExecuteAsync"/> is currently a placeholder: it reports that no command has been wired up yet and succeeds. It is meant to
/// be replaced with real command routing and dispatch once the command model exists, at which point it will depend on whatever registry that model
/// introduces instead of on a placeholder body.
/// </remarks>
internal sealed class CliCommandDispatcherService : BackgroundService
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliCommandDispatcherService"/> class.
    /// </summary>
    /// <param name="applicationLifetime">The application lifetime used to request host shutdown after the command has run.</param>
    /// <param name="logger">The logger used to report the command's outcome.</param>
    public CliCommandDispatcherService(IHostApplicationLifetime applicationLifetime, ILogger<CliCommandDispatcherService> logger)
    {
        this.applicationLifetime = applicationLifetime;
        this.logger = logger;
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The application lifetime used to signal that the host should shut down once the command has finished running.
    /// </summary>
    private readonly IHostApplicationLifetime applicationLifetime;

    /// <summary>
    /// The logger used to report the command's outcome, including any unhandled exception.
    /// </summary>
    private readonly ILogger<CliCommandDispatcherService> logger;

    #endregion

    #region Protected Methods

    /// <summary>
    /// Runs the command exactly once and then requests host shutdown. A cancellation (for example, from <c>Ctrl+C</c>) is reported with
    /// <see cref="ExitCode.Cancelled"/>, and any other unhandled exception is logged and reported with <see cref="ExitCode.GeneralError"/>, so that a
    /// failing command never leaves the process running or reports success.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that is signaled when the host begins shutting down.</param>
    /// <returns>Returns a task that represents the execution of the command.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Placeholder for the command routing and dispatch that the command model will introduce; until then, there is nothing to run, so this
            // reports that fact instead of executing anything
            this.logger.LogWarning("No command has been registered yet. Register the command model once it exists to execute commands.");
            Environment.ExitCode = ExitCode.Success;
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // A cancellation that originates from the host shutting down is an expected, orderly termination, reported with the conventional
            // cancellation exit code rather than as a failure.
            Environment.ExitCode = ExitCode.Cancelled;
        }
        catch (Exception exception)
        {
            this.logger.LogError(exception, "An unhandled exception occurred while executing the command.");
            Environment.ExitCode = ExitCode.GeneralError;
        }
        finally
        {
            // The command has finished (successfully, by cancellation, or by failure), so the host is told to shut down; without this the process
            // would keep running and wait for an external shutdown signal, which is the wrong behavior for a run-once CLI tool.
            this.applicationLifetime.StopApplication();
        }

        return Task.CompletedTask;
    }

    #endregion
}
