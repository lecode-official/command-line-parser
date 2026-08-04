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

[dprint](code-formatting-dprint.md), [CSpell](spell-checking-cspell.md), and [MarkdownLint](linting-markdownlint.md) are pinned as `devDependencies` in [`package.json`](../../../package.json) and locked in [`package-lock.json`](../../../package-lock.json). Install [Node.js](https://nodejs.org) (which bundles NPM), then, from the repository root:

```shell
npm ci
```

installs the exact versions locked in `package-lock.json` into `node_modules` (never `npm install`, which would happily update the lock file — see [Dependency Management](../conventions/dependency-management.md)). NPM prints a notice that `dprint`'s install scripts are not yet covered by the `allowScripts` field, since `dprint` uses a `postinstall` script to fetch its platform-specific binary. `npm approve-scripts dprint` reviews and approves it, but writes the approval to the shared, checked-in `package.json` — approving it for every other developer too, not just you.

To approve `dprint`'s install scripts for yourself only, without touching `package.json`, put an `allow-scripts` line in an `.npmrc` (`--allow-scripts` on the command line or as an environment variable is rejected outright for project-scoped installs):

```ini
allow-scripts=dprint
```

Either the user-level `~/.npmrc` (applies to every project on the machine, never touches this repository at all) or a project-level `.npmrc` at the repository root works. A project-level `.npmrc` is covered by [`.gitignore`](../../../.gitignore), to avoid accidentally committing a personal, unreviewed policy for other developers.

CSpell and MarkdownLint-cli2 are then available through `npx`:

```shell
npx cspell lint --config tests/linters/.cspell.json --no-progress "**/*"
npx markdownlint-cli2 --config tests/linters/.markdownlint.yml "**/*.md" "#**/build/**" "#**/node_modules/**"
```

`npx` resolves these straight to the pinned version in `node_modules/.bin`, so a local pass reliably predicts a green CI run; their Visual Studio Code extensions bundle their own engines, so no further install is needed for editor integration.

dprint is different: `npm ci` also installs it into `node_modules` (so `npx dprint check "**/*"` / `npx dprint fmt` work), but that alone is **not enough** for the editor — the Visual Studio Code dprint extension has no formatting engine of its own and requires `dprint` installed **globally, on the PATH**, regardless of any local, project-level install (see [Visual Studio Code Integration](vscode-integration.md)). Install it separately, pinned to the same version as `package.json`:

```shell
npm install --global dprint@0.55.1
```

## Python and ProperDocs

The [documentation website](documentation-website.md) is built with [ProperDocs](https://properdocs.org/), themed with [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/). Install [Python](https://www.python.org/downloads/) (which bundles `pip`), then create a virtual environment and install the pinned dependencies from [`requirements.txt`](../../../requirements.txt) into it, rather than into the global or user Python installation (see [Dependency Management](../conventions/dependency-management.md)):

```shell
python3 -m venv .venv
source .venv/bin/activate
python -m pip install --requirement requirements.txt
```

On Windows, activate with `.venv\Scripts\activate` instead of the `source` line above. `.venv` is already excluded in [`.gitignore`](../../../.gitignore), so it is never committed.

This installs ProperDocs itself, the Material theme, and the two plugins that generate the `docs/api` C# API reference and the site navigation, all pinned to the exact versions in `requirements.txt`, isolated to this project. Activate the virtual environment (`source .venv/bin/activate`) in every new shell before running `properdocs` or the other commands in [Documentation Website](documentation-website.md) — deactivate it with `deactivate` when done.

## Visual Studio Code

Visual Studio Code is recommended, though not required, to work on CLI.NET Core. When first opening the repository, Visual Studio Code offers to install the extensions recommended in [`.vscode/extensions.json`](../../../.vscode/extensions.json) — install all of them.

One of those, [Workspace Config Plus](https://marketplace.visualstudio.com/items?itemName=swellaby.workspace-config-plus), is not optional in practice: this project splits editor configuration into a checked-in `settings.shared.json` and a per-developer, git-ignored `settings.local.json`, and Workspace Config Plus is what merges the two into the `settings.json` Visual Studio Code actually reads. Without it installed, no `settings.json` is generated at all, so none of the shared configuration takes effect — format-on-save with dprint, the C# solution path, and the CSpell and MarkdownLint integrations all silently do nothing. See [Visual Studio Code Integration](vscode-integration.md) for the full shared/local mechanism and what each setting does.

## Related

- [Visual Studio Code Integration](vscode-integration.md) — the shared/local settings mechanism Workspace Config Plus enables.
- [Documentation Website](documentation-website.md) — running ProperDocs locally and how the `docs/api` reference is generated.
- [Continuous Integration](continuous-integration.md) — the pinned tool versions to match locally.
- [Dependency Management](../conventions/dependency-management.md) — how the SDK, local tool, and NuGet package versions are kept exact.
- [Testing and Code Coverage](testing-and-code-coverage.md) — running the tests and collecting coverage once the SDK is installed.
