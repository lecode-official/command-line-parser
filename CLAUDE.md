# CLAUDE.md

Guidance for AI assistants working on CLI.NET Core.

## What This Project Is

CLI.NET Core is a .NET command line application framework, built in the style of ASP.NET Core: it lets consumers define commands and command-line arguments much the same way they would define actions and parameters for a Web API. The repository holds one solution, [`source/CLI.NET Core.slnx`](source/CLI.NET%20Core.slnx), with two projects — the framework itself (`clinet-core`) and a sample app that references it (`sample-app`). Both target `net10.0` with nullable reference types and implicit usings enabled.

## Golden Rules

- **Match the surrounding code** This code is heavily and consistently documented — follow it.
- **Formatting is owned by dprint** Markdown, JSON, XML (including `.csproj`/`.slnx`), YAML, and TOML — never add a lint rule that reformats one of these. Run `dprint fmt` before finishing. **C# is not covered by dprint** (there is no C# plugin configured); its whitespace comes from [`.editorconfig`](.editorconfig), so review it by eye.
- **Run the linters before finishing** — there is no npm-script wrapper, so invoke them directly:

  ```shell
  dprint check "**/*"
  npx --yes cspell@10.0.1 lint --config tests/linters/.cspell.json --no-progress "**/*"
  npx --yes markdownlint-cli2@0.23.0 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"
  ```

  This file is Markdown and is linted too.
- **Markdown headings are title case** at every level, in every file. Preserve the real casing of code spans, brand names (`dprint`), and acronyms.
- **Prose uses periods, not semicolons.** In prose (docs, XML documentation comments, commit messages, this file) end each sentence with a period rather than joining two with a semicolon. Plain in-code comments (`//`) do the reverse: sentences are separated by semicolons and the last one takes no terminal punctuation.
- **C# structure**: file-scoped namespaces, an XML documentation comment (`<summary>`, `<param>`, `<returns>`) on every public and internal member, `<inheritdoc/>` for members that implement an interface or override a base member, `sealed` classes by default (composition over inheritance for anything that would otherwise need to extend a sealed framework type), explicit `this.` on member access, expression-bodied members where the implementation is a single expression, and `camelCase` private fields with no underscore.
- **Nullable reference types and implicit usings are enabled everywhere.** Write genuinely null-safe code rather than silencing the analyzer.
- **Files and directories are kebab-case**, except well-known and tool-mandated names (`README.md`, `LICENSE`, `.editorconfig`, ...). Inside a C# project's own source tree, directories and files switch to PascalCase, one type per file.
- **Every dependency is pinned to an exact version — never a range.** NuGet packages, dprint plugins, and the linter versions CI installs are all pinned exactly. Every C# project sets `RestorePackagesWithLockFile`, so a `PackageReference` version bump must be followed by `dotnet restore` and the resulting `packages.lock.json` change committed alongside it.
- **Write commit messages by the rules** — the 50/72 rule, a title-cased, past-tense subject, and a prose body. State whether AI was involved and, if it was, what exactly the AI did — this project is developed openly with AI assistance and the commit history is where that is tracked (see the "Use of AI" section of the [root README](README.md)). When you did any of the work, add the trailer this project uses (not a model-specific one):

  ```text
  Co-Authored-By: Claude <noreply@anthropic.com>
  ```

- **Delegating to subagents is pre-approved.** `.claude/settings.json` allows the agent/subagent tool by default, so use one whenever a task genuinely benefits from parallel or isolated work, without asking first.

## Conventions in Brief

Files use file-scoped namespaces and, in larger files, `#region` blocks (`Constructors`, `Private Fields`, `Public Methods`, and so on) to group members — small files skip regions entirely. Every public and internal member is documented with XML comments that explain *why*, not just what. Sealed types compose the framework types they wrap instead of inheriting from them. Markdown headings are title case everywhere, and prose ends sentences with periods; plain code comments do the reverse.
