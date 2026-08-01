# CLAUDE.md

Guidance for AI assistants working on CLI.NET Core. Humans should read the [documentation](docs/README.md) too.

## What This Project Is

CLI.NET Core is a .NET command line application framework, built in the style of ASP.NET Core: it lets consumers define commands and command-line arguments much the same way they would define actions and parameters for a Web API. The repository holds one solution, [`source/CLI.NET Core.slnx`](source/CLI.NET%20Core.slnx), with three projects — the framework itself (`clinet-core`), a sample app that references it (`sample-app`), and its xUnit unit test project (`tests/unit-tests`). All three target `net10.0` with nullable reference types and implicit usings enabled.

Start from [docs/developer-manual/architecture/overview.md](docs/developer-manual/architecture/overview.md).

## Golden Rules

- **Match the surrounding code and the documented conventions** ([docs/developer-manual/conventions/](docs/developer-manual/conventions/csharp-style.md)). This code is heavily and consistently documented — follow it.
- **Formatting is owned by dprint** ([docs/developer-manual/tooling/formatting-dprint.md](docs/developer-manual/tooling/formatting-dprint.md)) for Markdown, JSON, XML (including `.csproj`/`.slnx`), YAML, and TOML — never add a lint rule that reformats one of these. Run `dprint fmt` before finishing. **C# is not covered by dprint** (there is no C# plugin configured); its whitespace comes from [`.editorconfig`](.editorconfig) and its style from [C# Style](docs/developer-manual/conventions/csharp-style.md) alone, so review it by eye.
- **Run the linters before finishing** — there is no NPM-script wrapper, so invoke them directly with the versions [Continuous Integration](docs/developer-manual/tooling/continuous-integration.md) pins:

  ```shell
  dprint check "**/*"
  npx --yes cspell@10.0.1 lint --config tests/linters/.cspell.json --no-progress "**/*"
  npx --yes markdownlint-cli2@0.23.0 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"
  ```

  The docs and this file are Markdown and are linted too.
- **Markdown headings are title case** at every level, in every file ([docs/developer-manual/conventions/markdown-style.md](docs/developer-manual/conventions/markdown-style.md)). Preserve the real casing of code spans, brand names (`dprint`), and acronyms.
- **Wrap code in backticks.** Every type or member name, file or directory path, package name, configuration key, and CLI flag is wrapped in backticks wherever it appears in prose — docs and commit messages alike ([Markdown Style](docs/developer-manual/conventions/markdown-style.md#wrap-code-in-backticks)). XML documentation comments are the one exception: they are XML, not Markdown, so use `<see cref="..."/>` for identifiers and `<c>` for code instead ([C# Style](docs/developer-manual/conventions/csharp-style.md)).
- **Prose uses periods, not semicolons.** In prose (docs, XML documentation comments, commit messages, this file) end each sentence with a period rather than joining two with a semicolon. Plain in-code comments (`//`) do the reverse: sentences are separated by semicolons and the last one takes no terminal punctuation ([docs/developer-manual/conventions/csharp-style.md](docs/developer-manual/conventions/csharp-style.md)).
- **C# structure**: file-scoped namespaces, an XML documentation comment (`<summary>`, `<param>`, `<returns>`) on **every member, including `private` ones** (not just public and internal — this is for readers of the source, not only consumers of the compiled output), `<inheritdoc/>` for members that implement an interface or override a base member, `sealed` classes by default (composition over inheritance for anything that would otherwise need to extend a sealed framework type), explicit `this.` on member access, expression-bodied members where the implementation is a single expression, and `camelCase` private fields with no underscore. Full detail: [C# Style](docs/developer-manual/conventions/csharp-style.md).
- **Nullable reference types and implicit usings are enabled everywhere.** Write genuinely null-safe code rather than silencing the analyzer.
- **Files and directories are kebab-case**, except well-known and tool-mandated names (`README.md`, `LICENSE`, `.editorconfig`, ...). Inside a C# project's own source tree, directories and files switch to PascalCase, one type per file. Full detail: [File Naming Conventions](docs/developer-manual/conventions/file-naming-conventions.md).
- **Every dependency is pinned to an exact version — never a range.** NuGet packages, dprint plugins, the linter versions CI installs, and the .NET SDK (`global.json`), and local `dotnet` tools (`.config/dotnet-tools.json`) are all pinned exactly. Every C# project sets `RestorePackagesWithLockFile`, so a `PackageReference` version bump must be followed by `dotnet restore` and the resulting `packages.lock.json` change committed alongside it. Full detail: [Dependency Management](docs/developer-manual/conventions/dependency-management.md).
- **Every new feature is unit tested, in the same change.** A bug fix adds a test that reproduces the bug. Test classes live in `tests/unit-tests`, mirror the source namespace and folder they cover, and are named `<TypeName>Tests`; `internal` types are tested directly (the core project grants the test assembly `InternalsVisibleTo`). Full detail: [Testing](docs/developer-manual/conventions/testing.md). Run `dotnet test "source/CLI.NET Core.slnx"` before finishing; see [Testing and Code Coverage](docs/developer-manual/tooling/testing-and-code-coverage.md) for coverage collection.
- **Write commit messages by the rules** ([docs/developer-manual/conventions/commit-messages.md](docs/developer-manual/conventions/commit-messages.md)) — the 50/72 rule, a title-cased, past-tense subject, and a prose body. State whether AI was involved and, if it was, what exactly the AI did — this project is developed openly with AI assistance and the commit history is where that is tracked (see the "Use of AI" section of the [root README](README.md)). When you did any of the work, add the trailer this project uses (not a model-specific one):

  ```text
  Co-Authored-By: Claude <noreply@anthropic.com>
  ```

- **Delegating to subagents is pre-approved.** `.claude/settings.json` allows the agent/subagent tool by default, so use one whenever a task genuinely benefits from parallel or isolated work, without asking first.

## When Working on X, Read Y

- **Repository layout, the solution, the projects** → [docs/developer-manual/architecture/](docs/developer-manual/architecture/overview.md).
- **C# source code** → [docs/developer-manual/conventions/csharp-style.md](docs/developer-manual/conventions/csharp-style.md).
- **Installing the SDK, Node.js, or the editor before working on the project** → [docs/developer-manual/tooling/developer-setup.md](docs/developer-manual/tooling/developer-setup.md).
- **Linting, formatting, spell checking, CI, editor setup** → [docs/developer-manual/tooling/](docs/developer-manual/tooling/README.md).
- **Markdown and documentation style** → [docs/developer-manual/conventions/markdown-style.md](docs/developer-manual/conventions/markdown-style.md).
- **Writing commit messages** → [docs/developer-manual/conventions/commit-messages.md](docs/developer-manual/conventions/commit-messages.md).
- **Naming a new file or directory** → [docs/developer-manual/conventions/file-naming-conventions.md](docs/developer-manual/conventions/file-naming-conventions.md).
- **Adding or upgrading a dependency** → [docs/developer-manual/conventions/dependency-management.md](docs/developer-manual/conventions/dependency-management.md).
- **Writing or running unit tests, measuring code coverage** → [docs/developer-manual/conventions/testing.md](docs/developer-manual/conventions/testing.md) and [docs/developer-manual/tooling/testing-and-code-coverage.md](docs/developer-manual/tooling/testing-and-code-coverage.md).
- **Opening an issue, submitting a pull request, the AI-contribution policy, updating `CONTRIBUTORS.md`/`CHANGELOG.md`** → [CONTRIBUTING.md](CONTRIBUTING.md).
- **How consumers use the framework** → [docs/user-manual/](docs/user-manual/README.md).

## Conventions in Brief

Files use file-scoped namespaces and always group members into `#region` blocks (`Constructors`, `Private Fields`, `Public Methods`, and so on), even in small files, ordered constructors, constants, fields, properties, methods, then interface implementations (no access modifier in an interface-implementation region's name). Every member, including `private` ones, is documented with XML comments that explain *why*, not just what, and marks up keywords like `null` or `sealed` with `<see langword="..."/>`. Sealed types compose the framework types they wrap instead of inheriting from them. Markdown headings are title case everywhere, and prose ends sentences with periods; plain code comments do the reverse. Full detail: [docs/developer-manual/conventions/](docs/developer-manual/conventions/csharp-style.md).

## Keep the Documentation up to Date

**This is important.** Whenever you change, add, remove, or discover something about the project, update the relevant [`docs/`](docs/README.md) article **and** this `CLAUDE.md` in the same change:

- Keep the "When working on X" routing table above accurate.
- Keep the [docs index](docs/README.md), the [Developer Manual index](docs/developer-manual/README.md), and the [User Manual index](docs/user-manual/README.md) accurate — every article must be listed.
- A change to the public API updates the User Manual (how consumers use it), not only the Developer Manual (how the codebase itself is built) — the two documents serve different audiences, and the same change often needs a separate edit to each.
- If a change introduces a topic that does not fit an existing article, add a new small, single-topic article, link it from the relevant index, and reference it here if relevant.
- If a change invalidates something a doc says, fix the doc — do not leave it stale.

Treat the docs as part of the code: a change is not done until the docs and this file reflect it.
