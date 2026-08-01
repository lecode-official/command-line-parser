# Tooling

This section gives an overview of the tools used for development, including code editors and IDEs, linters, and code formatters:

1. [Developer Setup](developer-setup.md)
2. [Visual Studio Code Integration](vscode-integration.md)
3. [Spell Checking (CSpell)](spell-checking-cspell.md)
4. [MarkdownLint](linting-markdownlint.md)
5. [dprint](formatting-dprint.md)
6. [Continuous Integration](continuous-integration.md)
7. [Testing and Code Coverage](testing-and-code-coverage.md)

The linters and the code formatter are not installed as project dependencies (there is no Node.js package manifest in this repository) — they are run directly from the command line, either installed globally or invoked on demand through `npx`. [Continuous Integration](continuous-integration.md) installs pinned versions of all three explicitly; running them locally with the same versions keeps results consistent with what CI reports. The test project and its tools are the exception: they are restored the normal .NET way, as `PackageReference`s and a local tool manifest (see [Testing and Code Coverage](testing-and-code-coverage.md)).
