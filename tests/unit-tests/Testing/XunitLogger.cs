
#region Using Directives

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

#endregion

namespace CliNetCore.UnitTests.Testing;

/// <summary>
/// An <see cref="ILogger"/> that writes every log message it receives to an <see cref="ITestOutputHelper"/> instead of the console. Created by
/// <see cref="XunitLoggerProvider.CreateLogger"/>, one instance per category, exactly like the built-in console logger creates one instance of its
/// own logger type per category.
/// </summary>
public sealed class XunitLogger : ILogger
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitLogger"/> class.
    /// </summary>
    /// <param name="categoryName">The category this logger reports under, typically the fully qualified name of the type doing the logging.</param>
    /// <param name="output">The output helper every message this logger writes goes to.</param>
    public XunitLogger(string categoryName, ITestOutputHelper output)
    {
        this.categoryName = categoryName;
        this.output = output;
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// The category this logger reports under, written alongside every message so that a reader can tell which component logged it.
    /// </summary>
    private readonly string categoryName;

    /// <summary>
    /// The output helper every message this logger writes goes to.
    /// </summary>
    private readonly ITestOutputHelper output;

    #endregion

    #region ILogger Implementation

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        this.output.WriteLine($"[{logLevel}] {this.categoryName}: {formatter(state, exception)}");
        if (exception is not null)
            this.output.WriteLine(exception.ToString());
    }

    #endregion
}
