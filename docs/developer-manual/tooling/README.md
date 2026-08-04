# Tooling

This section gives an overview of the tools used for development, including code editors and IDEs, linters, and code formatters:

1. [Developer Setup](developer-setup.md)
2. [Visual Studio Code Integration](vscode-integration.md)
3. [Spell Checking (CSpell)](spell-checking-cspell.md)
4. [Markdown Linting (MarkdownLint)](linting-markdownlint.md)
5. [Code Formatting (dprint)](code-formatting-dprint.md)
6. [Documentation Website](documentation-website.md)
7. [Continuous Integration](continuous-integration.md)
8. [Testing and Code Coverage](testing-and-code-coverage.md)

The linters and the code formatter are pinned as `devDependencies` in [`package.json`](../../../package.json) at the repository root and locked in [`package-lock.json`](../../../package-lock.json), restored with `npm ci` and run through `npx` — dprint additionally needs a persistent global install for its Visual Studio Code extension (see [Developer Setup](developer-setup.md)). [Continuous Integration](continuous-integration.md) runs the same `npm ci` and `npx` invocations; running them locally keeps results consistent with what CI reports. The test project and its tools follow the equivalent .NET pattern instead, as `PackageReference`s and a local tool manifest (see [Testing and Code Coverage](testing-and-code-coverage.md)).
