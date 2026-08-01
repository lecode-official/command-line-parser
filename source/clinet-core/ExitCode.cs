
namespace CliNetCore;

/// <summary>
/// Defines the conventional process exit codes a CLI.NET Core application can return. Using these constants instead of raw numbers keeps exit codes
/// consistent across the hosting layer and, later, the command model, and gives each value a documented meaning callers can rely on.
/// </summary>
public static class ExitCode
{
    #region Public Constants

    /// <summary>
    /// Indicates that the command completed successfully.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Indicates that an unspecified error occurred while executing the command.
    /// </summary>
    public const int GeneralError = 1;

    /// <summary>
    /// Indicates a command line usage error, for example missing or malformed arguments.
    /// </summary>
    public const int UsageError = 64;

    /// <summary>
    /// Indicates that the command's input data was incorrect or malformed, as opposed to the command line arguments used to invoke it.
    /// </summary>
    public const int DataError = 65;

    /// <summary>
    /// Indicates that an input file named on the command line does not exist or cannot be read.
    /// </summary>
    public const int NoInput = 66;

    /// <summary>
    /// Indicates an internal error detected within the command's own logic, as opposed to a user, input, or environment error.
    /// </summary>
    public const int InternalSoftwareError = 70;

    /// <summary>
    /// Indicates that an output file the command needed to create could not be created.
    /// </summary>
    public const int CannotCreateOutput = 73;

    /// <summary>
    /// Indicates that an I/O error occurred while the command was executing.
    /// </summary>
    public const int IoError = 74;

    /// <summary>
    /// Indicates that a configuration file the command depends on was missing, unreadable, or contained invalid data.
    /// </summary>
    public const int ConfigError = 78;

    /// <summary>
    /// Indicates that the specified command was found but could not be executed, for example because it lacks execute permission or is not a
    /// valid executable.
    /// </summary>
    public const int CommandNotExecutable = 126;

    /// <summary>
    /// Indicates that the specified command could not be found.
    /// </summary>
    public const int CommandNotFound = 127;

    /// <summary>
    /// Indicates that the command was cancelled by the user, for example by pressing <c>Ctrl+C</c>.
    /// </summary>
    public const int Cancelled = 130;

    /// <summary>
    /// Indicates that the command's process was terminated by an unhandled <c>SIGKILL</c> signal (128 plus the signal number, 9).
    /// </summary>
    public const int Killed = 137;

    /// <summary>
    /// Indicates that the command's process was terminated by an unhandled <c>SIGTERM</c> signal (128 plus the signal number, 15).
    /// </summary>
    public const int Terminated = 143;

    /// <summary>
    /// Indicates that the command exited with a status outside the range a calling shell can interpret, or is otherwise used as a generic
    /// catch-all failure code.
    /// </summary>
    public const int OutOfRangeExitStatus = 255;

    #endregion
}
