# Overview

This article covers how the repository is laid out and how the .NET projects inside it are configured.

## Repository Layout

The repository is organized into a few top-level directories:

- [`source/`](../../../source/) — The source code of the CLI.NET Core framework itself, and the sample app that demonstrates it.
- [`tests/`](../../../tests/) — Unit and integration tests, plus the configuration for the linters and the code formatter (see [Tooling](../tooling/README.md)).
- [`docs/`](../../README.md) — This documentation.
- [`design/`](../../../design/) — Logo and brand assets, and the [design guide](../../../design/DESIGN.md) that governs them.

Top-level files round out the repository: [`README.md`](../../../README.md) is the project's front door, [`CHANGELOG.md`](../../../CHANGELOG.md) records what changed in each version, [`CONTRIBUTORS.md`](../../../CONTRIBUTORS.md) lists everyone who has contributed, and [`LICENSE`](../../../LICENSE) is the full text of the license (see [Contributing](../contributing.md) for how these are kept up to date). [`CONTRIBUTING.md`](../../../CONTRIBUTING.md), [`CODE_OF_CONDUCT.md`](../../../CODE_OF_CONDUCT.md), and [`SECURITY.md`](../../../SECURITY.md) round out the community-facing files GitHub recognizes by name.

## Solution and Projects

[`source/CLI.NET Core.slnx`](../../../source/CLI.NET%20Core.slnx) is the solution file, in the newer XML-based `.slnx` format rather than the classic `.sln` format. It groups two projects:

- **`clinet-core`** ([`CLI.NET Core.csproj`](../../../source/clinet-core/CLI.NET%20Core.csproj)) — The framework itself, packed and published as the `CliNetCore` NuGet package.
- **`sample-app`** ([`CLI.NET Core Sample App.csproj`](../../../source/sample-app/CLI.NET%20Core%20Sample%20App.csproj)) — A runnable sample application that references `clinet-core` via a project reference and demonstrates how the framework is used.

Both projects target `net10.0`, and both enable `<Nullable>` and `<ImplicitUsings>`. `clinet-core` additionally sets `<GenerateDocumentationFile>`, so that the XML documentation comments in the source (see [C# Style](../conventions/csharp-style.md)) ship alongside the compiled assembly and are available to consumers of the NuGet package through their editor's tooltips.

`clinet-core` depends on the `Microsoft.Extensions.Hosting` NuGet package — the same generic-host infrastructure ASP.NET Core itself builds on. This dependency is what lets CLI.NET Core mirror the ASP.NET Core hosting model for command-line applications, as described in the [root README](../../../README.md).

## Versioning and Licensing

The NuGet package version is set independently in each `.csproj`'s `<Version>` property. [`CHANGELOG.md`](../../../CHANGELOG.md) is the human-readable history of what each version changed; it is not generated from Git history, so it needs to be updated by hand (see [Contributing](../contributing.md)).

The project is licensed under LGPL-3.0 (see [`LICENSE`](../../../LICENSE)). Every `.csproj` mirrors this in its NuGet metadata.

## Related

- Coding conventions for the C# source: [C# Style](../conventions/csharp-style.md).
- Linters, the code formatter, and editor setup: [Tooling](../tooling/README.md).
- How to propose and submit changes: [Contributing](../contributing.md).
