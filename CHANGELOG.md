# Changelog

## v0.3.0

*To be determined.*

Complete rewrite as a .NET 10 library using the modern application hosting model and an API that mirrors ASP.NET Core.

In addition to the rewrite, the following changes were made:

- Since the permissive MIT license has recently been used more and more by large corporations to take from the open source community and not give anything back, the license of this project was changed from MIT to LGPL 3.0. The LGPL still allows people to freely use the library in their own software, even for commercial projects, but making any changes to it forces the derivative work to be released under the same license, thus forcing companies to give back to the community.
- The project now uses a set of linters and a code formatter to (1) find and correct problems in the code and (2) to consistently format all files. If you use Visual Studio Code to work on the project, all linters and the code formatter are configured to directly work in the code editor. A set of recommended extensions is included for the linters and the code formatter. The following tools are being used:
  - **CSpell**, a spell checker for code, to ensure that there are no misspellings in the code.
  - **MarkdownLint**, a linter for Markdown files, which ensures that the Markdown files are consistently formatted and standards are enforced.
  - **dprint**, an unopinionated, configurable code formatter with plugins for many languages. This code formatter is used to format Markdown, JSON, XML, YAML, and TOML files in the project.
- A completely new, modern logo was designed, which heavily borrows from the ASP.NET Core logo, with the "C" wordmark and a dot. The logo is available in both light and dark mode variants in the `design/` directory and a design guide for it was created.
- An xUnit unit test project was added at `tests/unit-tests`, covering the hosting layer, and code coverage is now collected and reported, locally with ReportGenerator and in a dedicated Continuous Integration workflow. From now on, every new feature is expected to come with unit tests in the same pull request.

## v0.2.0

*Never released.*

Complete rewrite as a .NET Core .NET Standard library with cross-platform support. Was never released and skipped.

## v0.1.1

*Released on July 1, 2016.*

Improved the error handling of the command line parser. Lexical and syntactical errors can now be handled.

## v0.1.0

*Released on June 15, 2016.*

The initial release of the command line parser.
