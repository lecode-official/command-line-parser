# Continuous Integration

This article covers the three GitHub Actions workflows in [`.github/workflows/`](../../../.github/workflows/): [`linters.yml`](../../../.github/workflows/linters.yml), [`tests.yml`](../../../.github/workflows/tests.yml), and [`docs.yml`](../../../.github/workflows/docs.yml). They are independent of each other — any one can fail, be re-run, or be extended without touching the others, and a commit or pull request shows them as separate checks.

## Linters Workflow

Runs on every push, to any branch. A single job, on the latest Ubuntu runner, checks out the repository, installs Node.js 24, then `npm ci` to install the exact versions of the three tools this project relies on for formatting and linting (pinned in [`package.json`](../../../package.json) and locked in [`package-lock.json`](../../../package-lock.json)), and runs each of them in turn through `npx`:

- `npx dprint check "**/*"` — verifies that every file dprint covers is already formatted (see [dprint](code-formatting-dprint.md)); unlike `dprint fmt`, it never modifies a file.
- `npx cspell lint --config tests/linters/.cspell.json --no-progress "**/*"` — spell-checks the whole repository (see [Spell Checking (CSpell)](spell-checking-cspell.md)).
- `npx markdownlint-cli2 --config tests/linters/.markdownlint.yml "**/*.md" "#**/build/**" "#**/node_modules/**"` — lints every Markdown file, excluding build output (see [MarkdownLint](linting-markdownlint.md)).

The spell-checking and Markdown-linting steps run even if an earlier step failed (`if: ${{ !cancelled() }}`), so a single failing tool does not hide findings from the others — the workflow always reports everything it can in one run.

`npm ci` fails instead of silently updating `package-lock.json` if it no longer matches `package.json` (see [Dependency Management](../conventions/dependency-management.md)). When running these tools locally (see the "Running It" section of each tool's article), the same `npm ci` plus `npx` gives a local run the exact versions CI uses, so a local pass reliably predicts a green CI run.

## Docs Workflow

Runs only on pushes to `main` — unlike the other two workflows, it deploys to GitHub Pages, so it only ever needs to run against what is actually published. A build job checks out the repository, sets up .NET and restores its local tools (for `xmldoc2md`), sets up Python and installs the pinned dependencies from [`requirements.txt`](../../../requirements.txt), and runs `properdocs build`, which regenerates the `docs/api` section from the framework's current XML documentation comments (see [Documentation Website](documentation-website.md)). A separate deploy job then publishes the built site to GitHub Pages through the `github-pages` environment.

## Tests Workflow

A single job, also on the latest Ubuntu runner, checks out the repository, installs the exact .NET SDK version pinned in [`global.json`](../../../global.json), restores the solution in `--locked-mode` (failing loudly rather than silently regenerating a `packages.lock.json`, see [Dependency Management](../conventions/dependency-management.md)), then builds and runs every test with code coverage collection enabled. It renders the coverage summary directly into the workflow run's summary page and uploads the full HTML report as an artifact. See [Testing and Code Coverage](testing-and-code-coverage.md) for the exact commands and what they produce, and [Testing](../conventions/testing.md) for what a change is expected to cover.

There is currently no enforced minimum coverage percentage that fails the build.

## Related

- [dprint](code-formatting-dprint.md), [Spell Checking (CSpell)](spell-checking-cspell.md), and [MarkdownLint](linting-markdownlint.md) — the three tools the linters workflow runs.
- [Documentation Website](documentation-website.md) — what the docs workflow builds and deploys, in more detail.
- [Testing and Code Coverage](testing-and-code-coverage.md) — the commands the tests workflow runs, in more detail.
- [Dependency Management](../conventions/dependency-management.md) — how the pinned SDK, tool, and package versions these workflows use are kept exact.
