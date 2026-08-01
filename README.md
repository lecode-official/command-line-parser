# ![CLI.NET Core](design/readme-header-dark.png#gh-dark-mode-only) ![CLI.NET Core](design/readme-header-light.png#gh-light-mode-only)

<div align="center">

[![Tests](https://github.com/lecode-official/command-line-parser/actions/workflows/tests.yml/badge.svg)](https://github.com/lecode-official/command-line-parser/actions/workflows/tests.yml) [![Linters](https://github.com/lecode-official/command-line-parser/actions/workflows/linters.yml/badge.svg)](https://github.com/lecode-official/command-line-parser/actions/workflows/linters.yml)

</div>

CLI.NET Core is a command line application framework in the style of ASP.NET Core. It allows you to define commands and command line arguments in much the same way you would define actions and arguments for a Web API in ASP.NET Core.

## Project Structure

The repository is organized into a few top-level directories:

- **`source/`** — The source code of the CLI.NET Core framework itself.
- **`tests/`** — The unit tests and integration tests that verify the behavior of the framework, as well as the linters and code formatters that enforce a consistent and high-quality codebase.
- **`docs/`** — The project documentation. Start with [`docs/README.md`](docs/README.md).
- **`design/`** — Design assets related to the project and the [design guide](design/DESIGN.md).

## Development

### Developer Setup

Building and working on CLI.NET Core requires the .NET SDK (the exact version pinned in [`global.json`](global.json)) and Node.js with NPM (used to run the linters and code formatter). Visual Studio Code is recommended, with the extensions recommended in [`.vscode/extensions.json`](.vscode/extensions.json) installed. See [Developer Setup](docs/developer-manual/tooling/developer-setup.md) for how to install and configure all of it.

### Code Formatting & Linting

The project uses a set of linters and a code formatter to (1) find and correct problems in the code and (2) to consistently format all files. If you use Visual Studio Code to work on the project, all linters and the code formatter are configured to directly work in the code editor. A set of recommended extensions is included for the linters and the code formatter. The following tools are being used:

- **[CSpell](tests/linters/.cspell.json)**, a spell checker for code, to ensure that there are no misspellings in the code.
- **[MarkdownLint](tests/linters/.markdownlint.yml)**, a linter for Markdown files, which ensures that the Markdown files are consistently formatted and standards are enforced.
- **[dprint](dprint.json)**, an unopinionated, configurable code formatter with plugins for many languages. This code formatter is used to format Markdown, JSON, XML, YAML, and TOML files in the project.

### Testing

The framework's behavior is verified by an xUnit unit test project, [`tests/unit-tests`](tests/unit-tests/). Every new feature is expected to come with unit tests covering it, in the same pull request (see [`CONTRIBUTING.md`](CONTRIBUTING.md)). Run the tests with:

```shell
dotnet test "source/CLI.NET Core.slnx"
```

See [the developer manual](docs/developer-manual/tooling/testing-and-code-coverage.md) for how code coverage is collected and reported.

## Use of AI

This project is developed with the help of AI and I want to be fully transparent about that. The heart of the project — its architecture, its public API, and the implementation of its core — was and will continue to be written by hand, by me. AI is a tool that I use for the work around that core, never a substitute for it, and everything an AI produces is reviewed and, where necessary, rewritten by me before it is committed. Concretely, I use AI for the following kinds of tasks:

- **Documentation** — Writing and revising the documentation, the read me files, and code comments.
- **Design assets** — Generating logo designs and other visual assets.
- **Boilerplate** — Generating repetitive, mechanical code, project scaffolding, and configuration files.
- **Difficult bugs** — Tracking down bugs where a second pair of eyes helps.
- **Architecture** — Discussing design and architecture decisions as a sounding board. The decisions themselves are always mine.

Every commit message states whether AI was involved and, if it was, what exactly the AI did. Such commits additionally carry a `Co-Authored-By: Claude <noreply@anthropic.com>` trailer, so that the extent of AI involvement can be traced through the Git history at any point. Commits without such a note were written entirely by hand.

This is my own policy for my own commits. If you are contributing to the project yourself, see [`CONTRIBUTING.md`](CONTRIBUTING.md) for the AI-assistance rules that apply to contributions.

## Contributing

If you'd like to contribute, there are multiple ways you can help out — bug reports, feature requests, or code. See [`CONTRIBUTING.md`](CONTRIBUTING.md) for the full process, including how AI-assisted contributions are handled. Participation is governed by the [Code of Conduct](CODE_OF_CONDUCT.md).

## Security

To report a security vulnerability, please do not open a public issue — see [`SECURITY.md`](SECURITY.md) for how to report it privately.

## License

The code in this project is licensed under the GNU Lesser General Public License v3.0 license. This generally means that you may use, distribute, and modify this work freely given that you make the complete source code of your modifications available under the same license. If your project uses this software only through the provided interfaces (i.e., you link against this library), your work may be distributed under different terms without having to publish the source code of your work, even if your project is closed source and commercial. If you make contributions you explicitly grant patent rights if your contribution includes anything patented by you. **This is no legal advice and you should read the [license file](LICENSE) for more information.**
