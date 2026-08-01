# Dependency Management

This article covers how dependencies — NuGet packages, but also the linters and code formatter from [Tooling](../tooling/README.md) — are pinned and locked in this repository.

## Exact Versions, Never Ranges

Every dependency is pinned to an exact version — never a floating range (`*`), a minimum-version range, or a wildcard. A `PackageReference` in a `.csproj` looks like this:

```xml
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
```

not `Version="10.*"` or `Version="[10.0.0,)"`. The same rule applies outside of NuGet: the dprint plugins in [`dprint.json`](../../../dprint.json) are pinned to an exact release URL, [Continuous Integration](../tooling/continuous-integration.md) installs `dprint`, `cspell`, and `markdownlint-cli2` at exact, explicit versions, and [`global.json`](../../../global.json) pins the exact .NET SDK version (with `"rollForward": "disable"`, so a newer SDK on a machine or runner is never silently substituted) that every `dotnet` command in this repository, including CI, builds with. An upgrade is always a deliberate, visible change to a version string, never something that happens silently on the next restore.

## Local .NET Tools

[`.config/dotnet-tools.json`](../../../.config/dotnet-tools.json) is a `dotnet` local tool manifest, restored with `dotnet tool restore`. It exists for tools that are invoked as part of the .NET build rather than through Node.js — currently just ReportGenerator, used to turn raw code-coverage data into a human-readable report (see [Testing and Code Coverage](../tooling/testing-and-code-coverage.md)). Like every other dependency, its version is pinned exactly; upgrading it is `dotnet tool update dotnet-reportgenerator-globaltool --version <exact version>`, followed by committing the resulting change to the manifest.

## Lock Files

Every C# project sets:

```xml
<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
```

which makes `dotnet restore` generate and verify a `packages.lock.json` file next to the project, recording the exact resolved version and content hash of every direct and transitive NuGet dependency. This file is committed to source control. With it in place, a restore fails loudly if the resolved dependency graph would differ from what is locked — for example because a transitive dependency's version range allows a newer release — instead of silently picking up a different set of packages than the last person who restored the project.

When a `PackageReference` version changes, run `dotnet restore` again to regenerate the affected `packages.lock.json` and commit it alongside the `.csproj` change; the two must move together.

## Related

- Where the `.csproj` files that set this live: [Architecture](../architecture/overview.md).
- The pinned tool versions used outside of NuGet: [Continuous Integration](../tooling/continuous-integration.md).
