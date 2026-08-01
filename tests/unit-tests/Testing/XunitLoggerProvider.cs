
#region Using Directives

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Testing;

/// <summary>
/// An <see cref="ILoggerProvider"/> that creates <see cref="XunitLogger"/> instances writing to a single <see cref="ITestOutputHelper"/>. xUnit only
/// surfaces <see cref="ITestOutputHelper"/> output for a failing test, or when the test runner is run verbosely, which is what makes registering this
/// provider, instead of the generic host's default console provider, the fix for <see cref="Microsoft.Extensions.Logging.ILogger"/>-based application
/// logging cluttering an otherwise clean <c>dotnet test</c> run. See <see cref="CliApplicationTestBase"/> for how a test class registers this
/// provider on a <see cref="CliNetCore.Application.CliApplicationBuilder"/> it builds.
/// </summary>
public sealed class XunitLoggerProvider : ILoggerProvider
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitLoggerProvider"/> class.
    /// </summary>
    /// <param name="output">The output helper every logger this provider creates writes to.</param>
    public XunitLoggerProvider(ITestOutputHelper output) => this.output = output;

    #endregion

    #region Private Fields

    /// <summary>
    /// The output helper every logger this provider creates writes to.
    /// </summary>
    private readonly ITestOutputHelper output;

    #endregion

    #region ILoggerProvider Implementation

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => new XunitLogger(categoryName, this.output);

    /// <inheritdoc/>
    public void Dispose()
    {
        // The ITestOutputHelper this provider writes to is owned, and disposed, by xUnit itself, not by this provider, so there is nothing of this
        // provider's own to release
    }

    #endregion
}
