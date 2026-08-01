# Visual Studio Code Integration

This article covers the Visual Studio Code setup under [`.vscode/`](../../../.vscode/).

## Shared and Local Configuration

The workspace configuration is split with the [Workspace Config Plus](https://marketplace.visualstudio.com/items?itemName=Swellaby.workspace-config-plus) extension (`Swellaby.workspace-config-plus`, recommended in [`extensions.json`](../../../.vscode/extensions.json)). For `settings` (and, if they are ever added, `tasks` and `launch`), the extension reads two source files and merges them into the file Visual Studio Code actually loads:

- `settings.shared.json` — checked into Git, applies to every developer.
- `settings.local.json` — per-developer, ignored by Git (see [`.gitignore`](../../../.gitignore)).
- `settings.json` — **generated** by the extension from the two files above.

The `settings.local.json` layer wins over `settings.shared.json`, so each developer can override or extend the shared config without touching it.

### Rules

- **Never edit the generated `settings.json`.** It is produced by the extension and overwritten on every change; any manual edit is lost. It is also Git-ignored — only `extensions.json` and the `*.shared.json` files are committed.
- **Change `settings.shared.json` only for things useful to every developer.** A shared change is committed and affects the whole team, so it must be genuinely common (a project-wide setting, a lint integration).
- **Everything personal goes in `settings.local.json`.** Anything specific to your machine or preference — icon themes, file-nesting patterns, other personal editor tweaks — belongs in the local layer, never in the shared one.

## Settings

[`settings.shared.json`](../../../.vscode/settings.shared.json) sets a 150-character editor ruler to match [dprint's line width](formatting-dprint.md), maps a few file names to the correct language for syntax highlighting, hides build output and OS clutter from the explorer and file watcher, protects the `main` and `development` branches from direct commits, imports the [CSpell](spell-checking-cspell.md) configuration and raises its diagnostic severity to error, points [MarkdownLint](linting-markdownlint.md) at the project configuration, enables format-on-save with [dprint](formatting-dprint.md), and points `dotnet.defaultSolution` at [`CLI.NET Core.slnx`](../../../source/CLI.NET%20Core.slnx) so a C# extension (OmniSharp or C# Dev Kit) does not have to guess which solution to load — without it, the solution's non-standard location (in `source/`, not the repository root) can lead the extension to miss part of the solution and report a file as not belonging to any project.

## Extensions

[`extensions.json`](../../../.vscode/extensions.json) recommends the extensions that back all of the above: `streetsidesoftware.code-spell-checker`, `DavidAnson.vscode-markdownlint`, `dprint.dprint`, and `Swellaby.workspace-config-plus`, plus a few editing conveniences (`TakumiI.markdowntable`, `wayou.vscode-todo-highlight`, `yzhang.markdown-all-in-one`) and `.gitignore` language support (`codezombiech.gitignore`).

The CSpell and MarkdownLint extensions bundle their own engines, so the version they use can differ slightly from the pinned version [Continuous Integration](continuous-integration.md) runs — both read the same configuration files, so results should still agree. The dprint extension does not bundle a formatting engine of its own; it uses the `dprint` executable available on the machine together with the plugins declared in [`dprint.json`](../../../dprint.json), so keeping a reasonably current `dprint` installation locally keeps the editor's formatting in step with CI.

## Related

- The tools these settings configure: [Spell Checking (CSpell)](spell-checking-cspell.md), [MarkdownLint](linting-markdownlint.md), [dprint](formatting-dprint.md).
- The pinned versions these extensions approximate: [Continuous Integration](continuous-integration.md).
