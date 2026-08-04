# Code Formatting (dprint)

This article covers code formatting, which is owned entirely by [dprint](https://dprint.dev).

Configuration lives in [`dprint.json`](../../../dprint.json). dprint formats Markdown, JSON, XML (including `.csproj` and `.slnx` files, which are explicitly associated with the XML plugin), YAML, and TOML — **not C#**, since no C# formatting plugin is configured. C# indentation instead comes from [`.editorconfig`](../../../.editorconfig) and is otherwise a matter of following [C# Style](../conventions/csharp-style.md) during review.

## Running It

dprint is pinned in [`package.json`](../../../package.json) and locked in [`package-lock.json`](../../../package-lock.json) like the other linters (see [Dependency Management](../conventions/dependency-management.md)), and `npm ci` installs it into `node_modules` — but the Visual Studio Code extension requires a separate, persistent **global** install regardless (see [Developer Setup](developer-setup.md)), so `dprint` is also available directly on the command line without `npx`:

```shell
dprint fmt
```

formats every file dprint is configured to handle, in place. To check formatting without modifying anything (what [Continuous Integration](continuous-integration.md) does):

```shell
dprint check "**/*"
```

If dprint is not installed globally, `npx dprint fmt` / `npx dprint check "**/*"` resolve to the same pinned version through `node_modules/.bin`.

Visual Studio Code is also configured to run dprint on save (see [Visual Studio Code Integration](vscode-integration.md)).

## dprint Owns Formatting — Linters Must Not

Because dprint is the single source of truth for formatting the file types it covers, the linters are configured to disable any rule that would reformat code. When adding a lint rule, never enable one that conflicts with dprint.

## Related

- Why C# is excluded and what governs its style instead: [C# Style](../conventions/csharp-style.md).
- The check-mode invocation CI uses: [Continuous Integration](continuous-integration.md).
