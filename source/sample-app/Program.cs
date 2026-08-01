
#region Using Directives

using CliNetCore.Application;

#endregion

// Creates the builder from the command-line arguments; this is the CLI analogue of ASP.NET Core's WebApplication.CreateBuilder(args) and gives access
// to the same Services, Configuration, and Logging surface for wiring up the application
CliApplicationBuilder builder = CliApplication.CreateBuilder(args);

// Builds and runs the application; RunAsync blocks until the command has finished executing and the host has shut down, at which point the process
// exits with the exit code the dispatcher stored in Environment.ExitCode. There is no command registered yet — the command model this sample will
// demonstrate is still being designed — so the dispatcher currently just reports that and exits successfully.
CliApplication application = builder.Build();
await application.RunAsync();
