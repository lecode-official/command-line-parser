
namespace CliNetCore.UnitTests;

/// <summary>
/// Tests the well-known process exit codes exposed by <see cref="ExitCode"/>. Their exact numeric values are effectively part of the framework's
/// public contract — shell scripts and calling processes branch on them — so each one is asserted individually to catch an accidental change to a
/// value, as opposed to a change to the member's presence, which the compiler would already catch.
/// </summary>
public sealed class ExitCodeTests
{
    #region Public Methods

    /// <summary>
    /// Verifies that <see cref="ExitCode.Success"/> is <c>0</c>, the POSIX convention for a process that completed successfully.
    /// </summary>
    [Fact]
    public void SuccessIs0() => Assert.Equal(0, ExitCode.Success);

    /// <summary>
    /// Verifies that <see cref="ExitCode.GeneralError"/> is <c>1</c>, the POSIX convention for an unspecified failure.
    /// </summary>
    [Fact]
    public void GeneralErrorIs1() => Assert.Equal(1, ExitCode.GeneralError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.UsageError"/> is <c>64</c>, the BSD <c>sysexits.h</c> convention for a command line usage error.
    /// </summary>
    [Fact]
    public void UsageErrorIs64() => Assert.Equal(64, ExitCode.UsageError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.DataError"/> is <c>65</c>, the BSD <c>sysexits.h</c> convention for incorrect or malformed input data.
    /// </summary>
    [Fact]
    public void DataErrorIs65() => Assert.Equal(65, ExitCode.DataError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.NoInput"/> is <c>66</c>, the BSD <c>sysexits.h</c> convention for an input file that does not exist or
    /// cannot be read.
    /// </summary>
    [Fact]
    public void NoInputIs66() => Assert.Equal(66, ExitCode.NoInput);

    /// <summary>
    /// Verifies that <see cref="ExitCode.InternalSoftwareError"/> is <c>70</c>, the BSD <c>sysexits.h</c> convention for an internal software
    /// error.
    /// </summary>
    [Fact]
    public void InternalSoftwareErrorIs70() => Assert.Equal(70, ExitCode.InternalSoftwareError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.CannotCreateOutput"/> is <c>73</c>, the BSD <c>sysexits.h</c> convention for an output file that could
    /// not be created.
    /// </summary>
    [Fact]
    public void CannotCreateOutputIs73() => Assert.Equal(73, ExitCode.CannotCreateOutput);

    /// <summary>
    /// Verifies that <see cref="ExitCode.IoError"/> is <c>74</c>, the BSD <c>sysexits.h</c> convention for an I/O error.
    /// </summary>
    [Fact]
    public void IoErrorIs74() => Assert.Equal(74, ExitCode.IoError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.ConfigError"/> is <c>78</c>, the BSD <c>sysexits.h</c> convention for a missing or invalid configuration
    /// file.
    /// </summary>
    [Fact]
    public void ConfigErrorIs78() => Assert.Equal(78, ExitCode.ConfigError);

    /// <summary>
    /// Verifies that <see cref="ExitCode.CommandNotExecutable"/> is <c>126</c>, the shell convention for a command that was found but could not
    /// be executed.
    /// </summary>
    [Fact]
    public void CommandNotExecutableIs126() => Assert.Equal(126, ExitCode.CommandNotExecutable);

    /// <summary>
    /// Verifies that <see cref="ExitCode.CommandNotFound"/> is <c>127</c>, the shell convention for a command that could not be found.
    /// </summary>
    [Fact]
    public void CommandNotFoundIs127() => Assert.Equal(127, ExitCode.CommandNotFound);

    /// <summary>
    /// Verifies that <see cref="ExitCode.Cancelled"/> is <c>130</c>, the shell convention for a process terminated by <c>Ctrl+C</c> (128 plus the
    /// <c>SIGINT</c> signal number, 2).
    /// </summary>
    [Fact]
    public void CancelledIs130() => Assert.Equal(130, ExitCode.Cancelled);

    /// <summary>
    /// Verifies that <see cref="ExitCode.Killed"/> is <c>137</c>, the shell convention for a process terminated by an unhandled <c>SIGKILL</c>
    /// signal (128 plus the signal number, 9).
    /// </summary>
    [Fact]
    public void KilledIs137() => Assert.Equal(137, ExitCode.Killed);

    /// <summary>
    /// Verifies that <see cref="ExitCode.Terminated"/> is <c>143</c>, the shell convention for a process terminated by an unhandled
    /// <c>SIGTERM</c> signal (128 plus the signal number, 15).
    /// </summary>
    [Fact]
    public void TerminatedIs143() => Assert.Equal(143, ExitCode.Terminated);

    /// <summary>
    /// Verifies that <see cref="ExitCode.OutOfRangeExitStatus"/> is <c>255</c>, the shell convention for an exit status outside the range a
    /// calling shell can interpret.
    /// </summary>
    [Fact]
    public void OutOfRangeExitStatusIs255() => Assert.Equal(255, ExitCode.OutOfRangeExitStatus);

    #endregion
}
