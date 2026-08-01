# Testing

This article covers the conventions for the unit tests in [`tests/unit-tests`](../../../tests/unit-tests/). For the tooling that runs them and measures code coverage, see [Testing and Code Coverage](../tooling/testing-and-code-coverage.md).

## Every New Feature Is Unit Tested

From this point forward, a change that implements a new feature is not done until it has unit tests covering it, in the same pull request. This applies to human and AI-assisted contributions alike (see [Contributing](../contributing.md)). A bug fix should also add a test that reproduces the bug and would fail without the fix. Documentation-only changes, formatting, and pure refactors that do not change behavior are exempt, but a refactor that changes behavior along the way is not.

Not every line needs to be covered for its own sake: a placeholder implementation that has no reachable failure path yet (for example a `catch` block nothing can currently throw into) does not need a contrived test written just to touch it. Test the behavior that exists, not the branches a future change will eventually make reachable.

## One Test Class per Source Type

Each type in `source/clinet-core` that has behavior worth verifying gets one test class, named `<TypeName>Tests`, in the mirroring namespace and folder: `CliNetCore.Application.CliApplication` is tested by `CliNetCore.UnitTests.Application.CliApplicationTests` in `tests/unit-tests/Application/CliApplicationTests.cs`. This mirrors how `source/clinet-core` itself is organized (see [Architecture](../architecture/overview.md)) so that a source file's tests are always easy to find.

## Internal Types Are Tested Directly

`clinet-core`'s `.csproj` grants the test assembly access to its internals via `InternalsVisibleTo`. An `internal` type with behavior of its own (for example `CliCommandDispatcherService`) is instantiated and exercised directly in its own test class, the same way a `public` type is, rather than only indirectly through whatever public surface happens to wrap it. Internal visibility is part of the design surface this project documents (see [C# Style](csharp-style.md)'s documentation-comment rule), so it deserves the same direct test coverage.

## Naming Test Methods

A test method name has three parts, run together as a single `PascalCase` identifier with no separator: the member under test, the scenario, and the expected outcome, for example `ArgumentsGivenNoArgumentsIsEmpty` or `RunAsyncNoCommandRegisteredCompletesAndReportsSuccess`. No member name in this codebase contains an underscore, test methods included (see [C# Style](csharp-style.md)) — a test explorer or failure message shows the full name on its own line regardless, so a separator is not needed to keep the three parts readable.

## Every Member Is Documented, Including Private Ones

A test class, and every member on it — test methods, test-double classes, and the members of those test-double classes — has an XML documentation comment, exactly like [C# Style](csharp-style.md) requires everywhere else, with no exception for `private` or for test code. A test method's name already states its scenario and expected outcome in short form (see above); the `<summary>` is not there to repeat that, it is there to explain, in prose, why the scenario matters and what the assertion is actually protecting against, for the benefit of someone reading the source who has not memorized the naming convention. `<inheritdoc/>` is used for a test double's interface members exactly as it would be for real production code (see [C# Style](csharp-style.md)). The test project also enables `GenerateDocumentationFile`, so a missing documentation comment on a public or internal member (every test class and test method, since xUnit requires them to be `public`) fails the build the same way it would in `clinet-core` — `private` members are not compiler-checked this way, so writing their documentation comments is a matter of discipline, not tooling.

## Structure

- Test classes and test-double classes are `sealed`, like every other class in this codebase by default (see [C# Style](csharp-style.md)).
- Members are still grouped into `#region` blocks — typically just `#region Public Methods` for the test methods, plus `#region Private Classes` when a test class defines a small fake or stub (see [C# Style](csharp-style.md)'s region-ordering rule) — even though a test class usually has only one region.
- A test body follows Arrange-Act-Assert, separated by a blank line between each part. No comment marks the three parts — the blank lines are enough, and a comment restating "arrange"/"act"/"assert" would violate the "do not restate what the code already says" rule (see [C# Style](csharp-style.md)).
- Prefer a real collaborator over a mock (for example, building a real `CliApplicationBuilder`/`CliApplication` and resolving services from it) whenever the real thing is cheap to construct, which it is throughout the application layer. Write a small hand-rolled fake (a private nested class implementing the interface) only for a dependency that cannot otherwise be observed, such as `IHostApplicationLifetime`.
- A test that touches genuinely process-global state (`Environment.ExitCode` is the current example) saves the original value and restores it in a `finally` block, so that one test's use of shared state cannot leak into another test or into the test runner's own exit code.

## Redirecting Application Logging to `ITestOutputHelper`

`Host.CreateApplicationBuilder`, which `CliApplicationBuilder` wraps, registers a console logging provider by default, so a test that runs a built `CliApplication` far enough for it to log something (for example, the `CliCommandDispatcherService` placeholder's warning) would otherwise print that message straight to the console, cluttering `dotnet test` output regardless of whether the test passed. `tests/unit-tests/Testing/` holds the fix: `XunitLogger` and `XunitLoggerProvider` are an `ILogger`/`ILoggerProvider` pair that write to an `ITestOutputHelper` instead, and `CliApplicationTestBase` is an unsealed base class that a test class derives from, taking an `ITestOutputHelper` through its constructor (xUnit injects a fresh one per test method) and exposing `CreateBuilder()`/`CreateBuilder(string[])` in place of `CliApplicationBuilder.CreateBuilder()`/`CliApplicationBuilder.CreateBuilder(string[])`, pre-wired with the provider. `ITestOutputHelper` output is only surfaced by the test runner for a failing test (or when run verbosely), so logging becomes available for diagnosis without printing unconditionally. `CliApplicationBuilderTests` and `CliApplicationTests` already derive from it; any new test class that builds a `CliApplicationBuilder` should too.

xUnit has no fixture mechanism (`IClassFixture<T>`, `ICollectionFixture<T>`) suited to this: both create one shared instance reused across multiple tests, but `ITestOutputHelper` is a fresh instance per test method, so a fixture built before it exists could never wrap the right one. Plain constructor injection into a base class, which xUnit re-instantiates every test method, is the correct mechanism instead.

## Related

- How to run the tests and generate a coverage report: [Testing and Code Coverage](../tooling/testing-and-code-coverage.md).
- The general C# conventions these tests also follow: [C# Style](csharp-style.md).
- Where the test project sits in the repository: [Architecture](../architecture/overview.md).
- The pull request requirement this article's first section states: [Contributing](../contributing.md).
