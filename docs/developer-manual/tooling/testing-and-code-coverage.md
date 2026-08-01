# Testing and Code Coverage

This article covers how to run the unit tests in [`tests/unit-tests`](../../../tests/unit-tests/) and how code coverage is measured, both locally and in [Continuous Integration](continuous-integration.md). For the conventions the tests themselves follow, see [Testing](../conventions/testing.md).

## The Test Project

`CLI.NET Core Unit Tests` ([`CLI.NET Core Unit Tests.csproj`](../../../tests/unit-tests/CLI.NET%20Core%20Unit%20Tests.csproj)) is an [xUnit.net](https://xunit.net) test project that references `clinet-core` via a project reference. Like the other two projects, it targets `net10.0` with `<Nullable>` and `<ImplicitUsings>` enabled, and sets `<RestorePackagesWithLockFile>` (see [Dependency Management](../conventions/dependency-management.md)). It also sets `<GenerateDocumentationFile>`, the same as `clinet-core`, so that a missing documentation comment on a test class or test method — every one of them is `public`, since xUnit requires that — fails the build (see [Testing](../conventions/testing.md) for what a test's documentation comment is expected to say). It is included in [`CLI.NET Core.slnx`](../../../source/CLI.NET%20Core.slnx) alongside `clinet-core` and `sample-app`.

## Running the Tests

```shell
dotnet test "source/CLI.NET Core.slnx"
```

runs every test in the solution. To run only the unit test project directly:

```shell
dotnet test "tests/unit-tests/CLI.NET Core Unit Tests.csproj"
```

## Measuring Code Coverage

Coverage is collected with the `coverlet.collector` data collector that ships as a `PackageReference` of the test project, through the cross-platform `XPlat Code Coverage` collector built into `Microsoft.NET.Test.Sdk`:

```shell
dotnet test "source/CLI.NET Core.slnx" --collect:"XPlat Code Coverage" --results-directory ./coverage
```

This writes a Cobertura-format `coverage.cobertura.xml` per test project into a new, randomly named subdirectory of `./coverage`. That raw XML is not meant to be read directly — turn it into an HTML report with [ReportGenerator](https://github.com/danielpalme/ReportGenerator), a pinned local `dotnet` tool declared in [`.config/dotnet-tools.json`](../../../.config/dotnet-tools.json) (see [Dependency Management](../conventions/dependency-management.md) for how local tools are pinned the same way NuGet packages are):

```shell
dotnet tool restore
dotnet tool run reportgenerator -reports:"coverage/**/coverage.cobertura.xml" -targetdir:coverage-report -reporttypes:Html
```

Open `coverage-report/index.html` to browse coverage by assembly, class, and line. Neither `coverage/` nor `coverage-report/` is committed — both are covered by [`.gitignore`](../../../.gitignore) — so it is safe to regenerate them at any time.

## In Continuous Integration

[`.github/workflows/tests.yml`](../../../.github/workflows/tests.yml) runs on every push, as its own workflow separate from [the linters workflow](continuous-integration.md), restoring, building, and testing the solution with coverage collection enabled the same way described above. It additionally renders the ReportGenerator summary directly into the workflow run's own summary page (via `MarkdownSummaryGithub` and `$GITHUB_STEP_SUMMARY`), so the coverage percentages are visible without downloading anything, and uploads the full HTML report as a workflow artifact for a closer look. There is currently no enforced minimum coverage percentage that fails the build — see [Testing](../conventions/testing.md) for what is expected of a change instead of a numeric threshold.

## Related

- The conventions the tests themselves follow: [Testing](../conventions/testing.md).
- How dependencies, including this local `dotnet` tool, are pinned: [Dependency Management](../conventions/dependency-management.md).
- The workflow that runs this in CI: [Continuous Integration](continuous-integration.md).
- Where the test project sits in the repository: [Architecture](../architecture/overview.md).
