
#region Using Directives

using CliNetCore.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Application;

/// <summary>
/// Tests <see cref="CliCommandDispatcherService"/> directly (it is <see langword="internal"/>, made accessible to this assembly via
/// <c>InternalsVisibleTo</c> on the core project), verifying its currently reachable success path: running once with no command registered reports
/// <see cref="ExitCode.Success"/> and requests that the host stop.
/// </summary>
public sealed class CliCommandDispatcherServiceTests
{
    #region Public Methods

    /// <summary>
    /// Verifies that running the dispatcher with no command registered reports <see cref="ExitCode.Success"/> and calls
    /// <see cref="IHostApplicationLifetime.StopApplication"/>. The placeholder body of <see cref="CliCommandDispatcherService.ExecuteAsync"/>
    /// currently has no cancellation or exception path reachable from the outside (see its documentation "remarks"), so only this success path is
    /// covered; the cancelled and failed paths become testable once real command dispatch replaces the placeholder.
    /// </summary>
    /// <returns>Returns a task that represents the asynchronous test.</returns>
    [Fact]
    public async Task ExecuteAsyncNoCommandRegisteredReportsSuccessAndStopsTheApplication()
    {
        FakeHostApplicationLifetime lifetime = new();
        CliCommandDispatcherService dispatcherService = new(lifetime, NullLogger<CliCommandDispatcherService>.Instance);

        int originalExitCode = Environment.ExitCode;
        try
        {
            await dispatcherService.StartAsync(CancellationToken.None);
            await (dispatcherService.ExecuteTask ?? Task.CompletedTask);

            Assert.Equal(ExitCode.Success, Environment.ExitCode);
            Assert.True(lifetime.StopApplicationCalled);
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
    }

    #endregion

    #region Private Classes

    /// <summary>
    /// A minimal, hand-written test double for <see cref="IHostApplicationLifetime"/>. None of the three lifetime tokens are ever actually signaled
    /// by anything in this fake, since <see cref="CliCommandDispatcherService"/>'s current placeholder body never observes them — only whether
    /// <see cref="StopApplication"/> was called is of interest to the test that uses this class.
    /// </summary>
    private sealed class FakeHostApplicationLifetime : IHostApplicationLifetime
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether <see cref="StopApplication"/> has been called at least once. This is the one piece of observable state
        /// this fake adds beyond the interface it implements, and is what the test using it actually asserts against.
        /// </summary>
        public bool StopApplicationCalled { get; private set; }

        #endregion

        #region IHostApplicationLifetime Implementation

        /// <inheritdoc/>
        public CancellationToken ApplicationStarted => CancellationToken.None;

        /// <inheritdoc/>
        public CancellationToken ApplicationStopping => CancellationToken.None;

        /// <inheritdoc/>
        public CancellationToken ApplicationStopped => CancellationToken.None;

        /// <inheritdoc/>
        public void StopApplication() => this.StopApplicationCalled = true;

        #endregion
    }

    #endregion
}
