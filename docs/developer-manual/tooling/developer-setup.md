# Developer Setup

This article covers what to install before building, testing, linting, or formatting CLI.NET Core locally.

## .NET SDK

Install the exact .NET SDK version pinned in [`global.json`](../../../global.json) (currently `10.0.300`) from the [.NET downloads page](https://dotnet.microsoft.com/download). `global.json` sets `rollForward` to `disable`, so a mismatched version fails outright rather than silently substituting a different one — see [Dependency Management](../conventions/dependency-management.md).

Once the SDK is installed, restore the pinned local `dotnet` tools declared in [`.config/dotnet-tools.json`](../../../.config/dotnet-tools.json):

```shell
dotnet tool restore
```

This currently installs [ReportGenerator](https://github.com/danielpalme/ReportGenerator), used to turn raw code coverage output into an HTML report (see [Testing and Code Coverage](testing-and-code-coverage.md)).

## Node.js and NPM

[dprint](formatting-dprint.md), [CSpell](spell-checking-cspell.md), and [MarkdownLint](linting-markdownlint.md) are not installed as project dependencies (there is no Node.js package manifest in this repository) — install [Node.js](https://nodejs.org) (which bundles NPM), then:

- Install dprint globally, pinned to the version [Continuous Integration](continuous-integration.md) uses:

  ```shell
  npm install --global dprint@0.55.1
  ```

  A persistent install is required here, not just for the `dprint check "**/*"` and `dprint fmt` commands, but because the Visual Studio Code dprint extension has no formatting engine of its own — it calls the `dprint` executable on the machine directly (see [Visual Studio Code Integration](vscode-integration.md)).
- CSpell and MarkdownLint-cli2 do not need a persistent install — run them on demand through `npx`, pinned to the versions CI uses:

  ```shell
  npx --yes cspell@10.0.1 lint --config tests/linters/.cspell.json --no-progress "**/*"
  npx --yes markdownlint-cli2@0.23.0 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"
  ```

  `npx` downloads and caches the pinned version the first time it runs, so a local pass reliably predicts a green CI run. Their Visual Studio Code extensions bundle their own engines, so no global install is needed for editor integration either.

## Visual Studio Code

Visual Studio Code is recommended, though not required, to work on CLI.NET Core. When first opening the repository, Visual Studio Code offers to install the extensions recommended in [`.vscode/extensions.json`](../../../.vscode/extensions.json) — install all of them.

One of those, [Workspace Config Plus](https://marketplace.visualstudio.com/items?itemName=swellaby.workspace-config-plus), is not optional in practice: this project splits editor configuration into a checked-in `settings.shared.json` and a per-developer, git-ignored `settings.local.json`, and Workspace Config Plus is what merges the two into the `settings.json` Visual Studio Code actually reads. Without it installed, no `settings.json` is generated at all, so none of the shared configuration takes effect — format-on-save with dprint, the C# solution path, and the CSpell and MarkdownLint integrations all silently do nothing. See [Visual Studio Code Integration](vscode-integration.md) for the full shared/local mechanism and what each setting does.

## Related

- [Visual Studio Code Integration](vscode-integration.md) — the shared/local settings mechanism Workspace Config Plus enables.
- [Continuous Integration](continuous-integration.md) — the pinned tool versions to match locally.
- [Dependency Management](../conventions/dependency-management.md) — how the SDK, local tool, and NuGet package versions are kept exact.
- [Testing and Code Coverage](testing-and-code-coverage.md) — running the tests and collecting coverage once the SDK is installed.
