# Architecture

This article covers how the repository is laid out and how the .NET projects inside it are configured.

## Repository Layout

The repository is organized into a few top-level directories:

- [`source/`](../../source/) — The source code of the CLI.NET Core framework itself, and the sample app that demonstrates it.
- [`tests/`](../../tests/) — Unit and integration tests, plus the configuration for the linters and the code formatter (see [Tooling](tooling/README.md)).
- [`docs/`](../README.md) — This documentation, including the [Documentation Website](tooling/documentation-website.md)'s build scripts (`docs/build/`) and the one theme template it overrides (`docs/assets/templates/`), both wired in via [`properdocs.yml`](../../properdocs.yml).
- [`design/`](../../design/) — Logo and brand assets; see the [Logo Design](logo-design.md) guide that governs them.

Top-level files round out the repository: [`README.md`](../../README.md) is the project's front door, [`CHANGELOG.md`](../../CHANGELOG.md) records what changed in each version, [`CONTRIBUTORS.md`](../../CONTRIBUTORS.md) lists everyone who has contributed, and [`LICENSE`](../../LICENSE) is the full text of the license (see [Contributing](contribution-guide.md) for how these are kept up to date). [`CONTRIBUTING.md`](../../CONTRIBUTING.md), [`CODE_OF_CONDUCT.md`](../../CODE_OF_CONDUCT.md), and [`SECURITY.md`](../../SECURITY.md) round out the community-facing files GitHub recognizes by name. [`global.json`](../../global.json) pins the exact .NET SDK version the repository builds with, and [`.config/dotnet-tools.json`](../../.config/dotnet-tools.json) is the local tool manifest that pins ReportGenerator, the coverage-report tool (see [Testing and Code Coverage](tooling/testing-and-code-coverage.md)) — both follow the same exact-version-only rule as everything else (see [Dependency Management](conventions/dependency-management.md)).

## Solution and Projects

[`source/CLI.NET Core.slnx`](../../source/CLI.NET%20Core.slnx) is the solution file, in the newer XML-based `.slnx` format rather than the classic `.sln` format. It groups three projects:

- **`clinet-core`** ([`CLI.NET Core.csproj`](../../source/clinet-core/CLI.NET%20Core.csproj)) — The framework itself, packed and published as the `CliNetCore` NuGet package.
- **`sample-app`** ([`CLI.NET Core Sample App.csproj`](../../source/sample-app/CLI.NET%20Core%20Sample%20App.csproj)) — A runnable sample application that references `clinet-core` via a project reference and demonstrates how the framework is used.
- **`unit-tests`** ([`CLI.NET Core Unit Tests.csproj`](../../tests/unit-tests/CLI.NET%20Core%20Unit%20Tests.csproj)) — The xUnit test project that verifies `clinet-core`'s behavior, living under [`tests/`](../../tests/) rather than `source/` (see [Testing and Code Coverage](tooling/testing-and-code-coverage.md)).

All three projects target `net10.0`, and all three enable `<Nullable>` and `<ImplicitUsings>`. `clinet-core` additionally sets `<GenerateDocumentationFile>`, so that the XML documentation comments in the source (see [C# Style](conventions/csharp-style.md)) ship alongside the compiled assembly and are available to consumers of the NuGet package through their editor's tooltips, and grants the test project access to its `internal` members via `InternalsVisibleTo`, so internal types can be unit tested directly (see [Testing](conventions/testing.md)).

`clinet-core` depends on the `Microsoft.Extensions.Hosting` NuGet package — the same generic-host infrastructure ASP.NET Core itself builds on. This dependency is what lets CLI.NET Core mirror the ASP.NET Core hosting model for command-line applications, as described in the [root README](../../README.md).

## Build Output

Two `Directory.Build.props` files redirect every project's `bin`/`obj` output out of its own folder and into the repository root's `build/<project folder name>/` instead — `build/clinet-core/`, `build/sample-app/`, and `build/unit-tests/` — rather than a single one at the repository root, so that root stays uncluttered:

- [`source/Directory.Build.props`](../../source/Directory.Build.props) covers `clinet-core` and `sample-app`, using `$([System.IO.Path]::GetFileName($(MSBuildProjectDirectory)))` to read each project's own folder name at evaluation time, so the one file covers both projects without listing them individually.
- [`tests/unit-tests/Directory.Build.props`](../../tests/unit-tests/Directory.Build.props) covers `unit-tests` alone, so it hard-codes `build/unit-tests/` instead — there is only the one project under `tests/`, so the folder-name lookup above would be needless indirection here.

The documentation website builds into `build/documentation-website/` alongside them, via `site_dir` in [`properdocs.yml`](../../properdocs.yml) (see [Documentation Website](tooling/documentation-website.md)). The entire `build/` directory is generated and covered by [`.gitignore`](../../.gitignore) — nothing under it is ever committed.

## Versioning and Licensing

The NuGet package version is set independently in each `.csproj`'s `<Version>` property. [`CHANGELOG.md`](../../CHANGELOG.md) is the human-readable history of what each version changed; it is not generated from Git history, so it needs to be updated by hand (see [Contributing](contribution-guide.md)).

The project is licensed under LGPL-3.0 (see [`LICENSE`](../../LICENSE)). Every `.csproj` mirrors this in its NuGet metadata.

## Related

- Coding conventions for the C# source: [C# Style](conventions/csharp-style.md).
- Linters, the code formatter, and editor setup: [Tooling](tooling/README.md).
- Running the unit tests and measuring code coverage: [Testing and Code Coverage](tooling/testing-and-code-coverage.md).
- How to propose and submit changes: [Contributing](contribution-guide.md).
