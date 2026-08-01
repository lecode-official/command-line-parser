# File Naming Conventions

This article covers how files and directories are named throughout the repository.

## Kebab-Case by Default

Files and directories are named in **kebab-case** — lowercase words separated by hyphens — for example `command-line-parser`, `sample-app`, `developer-manual`, `formatting-dprint.md`. This applies repository-wide: source directories at the top level, the `docs/` tree, and any Markdown article.

## Well-Known Exceptions

Files with a conventional, tool- or ecosystem-mandated name keep that name instead of being renamed to kebab-case. This covers two kinds of files:

- **Files everyone recognizes by their standard name**: `README.md`, `CLAUDE.md`, `LICENSE`, `CHANGELOG.md`, `CONTRIBUTORS.md`, `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, `SECURITY.md`.
- **Files whose name is dictated by the tool that reads them**: `.editorconfig`, `.gitignore`, `.gitattributes`, `dprint.json`, `.cspell.json`, `.markdownlint.yml`, and so on. Renaming these would simply stop the tool from finding them.

## PascalCase Inside C# Projects

Inside a C# project's own source tree, directories and files switch to **PascalCase** — for example `source/clinet-core/Application/CliApplication.cs`. This follows the standard .NET convention of naming a file after the single type it contains, and grouping related types into a PascalCase namespace folder. The project's own top-level folder (`clinet-core`, `sample-app`) still follows the repository's kebab-case rule — the switch to PascalCase only happens once you are inside the project, looking at its namespaces and types.

The project and solution files themselves (`CLI.NET Core.csproj`, `CLI.NET Core Sample App.csproj`, `CLI.NET Core.slnx`) are named after the product, spaces included, rather than either convention — see [Architecture](../architecture/overview.md) for where they live.

## Related

- Where these directories and projects sit in the repository: [Architecture](../architecture/overview.md).
- The naming rules that apply once you are inside a C# file: [C# Style](csharp-style.md).
