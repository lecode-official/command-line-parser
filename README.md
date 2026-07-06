# CLI.NET Core

![CLI.NET Core Logo](design/readme-header-dark.png#gh-dark-mode-only) ![CLI.NET Core Logo](design/readme-header-light.png#gh-light-mode-only)

CLI.NET Core is a command line application framework in the style of ASP.NET Core. It allows you to define commands and command line arguments in much the same way you would define actions and arguments for a Web API in ASP.NET Core.

## Project Structure

The repository is organized into a few top-level directories:

- **`source/`** — The source code of the CLI.NET Core framework itself.
- **`tests/`** — The unit tests and integration tests that verify the behavior of the framework, as well as the linters and code formatters that enforce a consistent and high-quality codebase.
- **`docs/`** — The project documentation. Start with [`docs/README.md`](docs/README.md).
- **`design/`** — Design assets related to the project and the [design guide](design/DESIGN.md).

## Development

### Code Formatting & Linting

The project uses a set of linters and a code formatter to (1) find and correct problems in the code and (2) to consistently format all files. If you use Visual Studio Code to work on the project, all linters and the code formatter are configured to directly work in the code editor. A set of recommended extensions is included for the linters and the code formatter. The following tools are being used:

- **[CSpell](tests/linters/.cspell.json)**, a spell checker for code, to ensure that there are no misspellings in the code.
- **[MarkdownLint](tests/linters/.markdownlint.yml)**, a linter for Markdown files, which ensures that the Markdown files are consistently formatted and standards are enforced.
- **[dprint](dprint.json)**, an unopinionated, configurable code formatter with plugins for many languages. This code formatter is used to format Markdown, JSON, XML, YAML, and TOML files in the project.

## Contributing

If you'd like to contribute, there are multiple ways you can help out. If you find a bug or have a feature request, please feel free to open an issue on GitHub. If you want to contribute code, please fork the repository and use a feature branch. Pull requests are always welcome. Before forking, please open an issue where you describe what you want to do. This helps to align your ideas with mine and may prevent you from doing work, that I am already planning on doing. If you have contributed to the project, please add yourself to the [contributors list](CONTRIBUTORS.md) and document your changes in the [changelog](CHANGELOG.md). Also, if necessary, update the [documentation](docs/README.md). To help speed up the merging of your pull request, please comment and document your code extensively and try to emulate the coding style of the project.

## License

The code in this project is licensed under the GNU Lesser General Public License v3.0 license. This generally means that you may use, distribute, and modify this work freely given that you make the complete source code of your modifications available under the same license. If your project uses this software only through the provided interfaces (i.e., you link against this library), your work may be distributed under different terms without having to publish the source code of your work, even if your project is closed source and commercial. If you make contributions you explicitly grant patent rights if your contribution includes anything patented by you. **This is no legal advice and you should read the [license file](LICENSE) for more information.**
