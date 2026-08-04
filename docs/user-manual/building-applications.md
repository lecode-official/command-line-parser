# Building Applications

This article walks through building a command line application with CLI.NET Core, from creating the application to handling its exit code. It follows the same steps a new console project would take, in order.

## Create the Application

Start from a plain .NET console project (`dotnet new console`) that references the `CliNetCore` NuGet package. In `Program.cs`, create a builder from the command-line arguments your `Main` method (or top-level statements) received:

```csharp
using CliNetCore.Application;

CliApplicationBuilder builder = CliApplication.CreateBuilder(args);
```

`CliApplication.CreateBuilder(args)` is the CLI.NET Core analogue of ASP.NET Core's `WebApplication.CreateBuilder(args)`. Pass it the same `args` array your program was started with. If you don't have arguments to pass (for example, in a test), call the parameterless `CliApplication.CreateBuilder()` instead — it behaves the same way but with no command-line arguments.

## Configure Services, Configuration, and Logging

Before building the application, `builder` gives you the same configuration surface an ASP.NET Core host gives you, because `CliApplicationBuilder` implements `IHostApplicationBuilder`:

```csharp
builder.Services.AddSingleton<IGreeter, Greeter>();
builder.Logging.AddConsole();
string? greeting = builder.Configuration["Greeting"];
```

Use `builder.Services` to register your own services with dependency injection, `builder.Configuration` to read configuration, and `builder.Logging` to configure logging, exactly as you would in an ASP.NET Core application. Anything you register here is available once the application starts.

## Read the Command-Line Arguments

If a service you register needs the raw command-line arguments, depend on `CliCommandLineArguments` instead of calling `Environment.GetCommandLineArgs()` directly:

```csharp
public sealed class Greeter : IGreeter
{
    public Greeter(CliCommandLineArguments commandLineArguments) => this.commandLineArguments = commandLineArguments;

    private readonly CliCommandLineArguments commandLineArguments;

    public void Greet()
    {
        foreach (string argument in this.commandLineArguments.Arguments)
        {
            Console.WriteLine(argument);
        }
    }
}
```

`CliCommandLineArguments` is always registered for you, so you never need to register it yourself and never need to guard against it being missing. Its `Arguments` property holds the arguments in the order they were given, and is an empty list — never `null` — when the application was created without arguments.

## Build and Run the Application

Once you are done configuring the builder, call `Build()` to produce the application, then run it:

```csharp
CliApplication application = builder.Build();
await application.RunAsync();
```

`Build()` freezes configuration: after it returns, the service provider has been built and no more services can be registered. `RunAsync()` (or its blocking counterpart, `Run()`) starts the application and does not return until it shuts down, which today happens automatically as soon as the current command has finished executing.

Put together, a minimal `Program.cs` looks like this:

```csharp
using CliNetCore.Application;

CliApplicationBuilder builder = CliApplication.CreateBuilder(args);
builder.Services.AddSingleton<IGreeter, Greeter>();

CliApplication application = builder.Build();
await application.RunAsync();
```

## Handle the Exit Code

When the application shuts down, the process exits with a code from `CliNetCore.ExitCode`:

| Constant                         | Value | Meaning                                                                            |
| -------------------------------- | ----- | ---------------------------------------------------------------------------------- |
| `ExitCode.Success`               | `0`   | The command completed successfully.                                                |
| `ExitCode.GeneralError`          | `1`   | An unspecified error occurred while executing the command.                         |
| `ExitCode.UsageError`            | `64`  | The command line was used incorrectly, for example missing or malformed arguments. |
| `ExitCode.DataError`             | `65`  | The command's input data was incorrect or malformed.                               |
| `ExitCode.NoInput`               | `66`  | An input file named on the command line does not exist or cannot be read.          |
| `ExitCode.InternalSoftwareError` | `70`  | An internal error was detected within the command's own logic.                     |
| `ExitCode.CannotCreateOutput`    | `73`  | An output file the command needed to create could not be created.                  |
| `ExitCode.IoError`               | `74`  | An I/O error occurred while the command was executing.                             |
| `ExitCode.ConfigError`           | `78`  | A configuration file the command depends on was missing, unreadable, or invalid.   |
| `ExitCode.CommandNotExecutable`  | `126` | The specified command was found but could not be executed.                         |
| `ExitCode.CommandNotFound`       | `127` | The specified command could not be found.                                          |
| `ExitCode.Cancelled`             | `130` | The command was cancelled by the user, for example by pressing `Ctrl+C`.           |
| `ExitCode.Killed`                | `137` | The command's process was terminated by an unhandled `SIGKILL` signal.             |
| `ExitCode.Terminated`            | `143` | The command's process was terminated by an unhandled `SIGTERM` signal.             |
| `ExitCode.OutOfRangeExitStatus`  | `255` | The command exited with a status outside the range a calling shell can interpret.  |

None of these are reserved for a particular layer of the framework — they're ordinary `int` constants, and any command or application code can return any of them. Today, the framework itself only sets three automatically: cancelling the application (for example with `Ctrl+C`) reports `ExitCode.Cancelled`, an unhandled exception reports `ExitCode.GeneralError`, and otherwise the application reports `ExitCode.Success`. The rest, including `ExitCode.UsageError` and `ExitCode.CommandNotFound`, are there for you to return yourself; some of them will simply read more naturally once the command model described below exists to produce them on your behalf.

## What's Missing Today

CLI.NET Core does not yet have a command model: there is no way to define individual commands or bind their arguments to strongly typed parameters, so an application built today only reaches the bootstrap-and-exit behavior described above. Once the command model exists, this article will grow with the articles that describe it.

## Related

- How the framework's projects and packages are laid out: [Architecture](../developer-manual/architecture.md).
- Back to the [User Manual](README.md).
