
namespace CliNetCore.Application;

/// <summary>
/// Holds the raw command-line arguments the application was started with, resolved from the dependency-injection container. This lets command
/// logic depend on the arguments the same way it depends on any other service, instead of reading <see cref="Environment.GetCommandLineArgs"/>
/// directly. It is always registered, even when the application was created without any arguments, so that consumers never need to handle a
/// missing registration.
/// </summary>
public sealed class CliCommandLineArguments
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CliCommandLineArguments"/> class.
    /// </summary>
    /// <param name="arguments">The command-line arguments the application was started with.</param>
    internal CliCommandLineArguments(IReadOnlyList<string> arguments) => this.Arguments = arguments;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the command-line arguments the application was started with, in the order they were given. This is empty, never <see langword="null"/>,
    /// when the application was created without arguments.
    /// </summary>
    public IReadOnlyList<string> Arguments { get; }

    #endregion
}
