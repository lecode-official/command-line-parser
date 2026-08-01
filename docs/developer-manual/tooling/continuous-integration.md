# Continuous Integration

This article covers the two GitHub Actions workflows in [`.github/workflows/`](../../../.github/workflows/): [`linters.yml`](../../../.github/workflows/linters.yml) and [`tests.yml`](../../../.github/workflows/tests.yml). Both run on every push, to any branch, and are independent of each other — either can fail, be re-run, or be extended without touching the other, and a commit or pull request shows them as two separate checks.

## Linters Workflow

A single job, on the latest Ubuntu runner, checks out the repository, installs Node.js 24, installs pinned versions of the three tools this project relies on for formatting and linting, and then runs each of them in turn:

- `dprint check "**/*"` — verifies that every file dprint covers is already formatted (see [dprint](formatting-dprint.md)); unlike `dprint fmt`, it never modifies a file.
- `cspell lint --config tests/linters/.cspell.json --no-progress "**/*"` — spell-checks the whole repository (see [Spell Checking (CSpell)](spell-checking-cspell.md)).
- `markdownlint-cli2 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"` — lints every Markdown file, excluding build output (see [MarkdownLint](linting-markdownlint.md)).

The spell-checking and Markdown-linting steps run even if an earlier step failed (`if: ${{ !cancelled() }}`), so a single failing tool does not hide findings from the others — the workflow always reports everything it can in one run.

The tool versions this workflow installs (`dprint`, `cspell`, `markdownlint-cli2`) are pinned exactly. When running these tools locally (see the "Running It" section of each tool's article), use the same pinned versions so that a local pass reliably predicts a green CI run.

## Tests Workflow

A single job, also on the latest Ubuntu runner, checks out the repository, installs the exact .NET SDK version pinned in [`global.json`](../../../global.json), restores the solution in `--locked-mode` (failing loudly rather than silently regenerating a `packages.lock.json`, see [Dependency Management](../conventions/dependency-management.md)), then builds and runs every test with code coverage collection enabled. It renders the coverage summary directly into the workflow run's summary page and uploads the full HTML report as an artifact. See [Testing and Code Coverage](testing-and-code-coverage.md) for the exact commands and what they produce, and [Testing](../conventions/testing.md) for what a change is expected to cover.

There is currently no enforced minimum coverage percentage that fails the build.

## Related

- [dprint](formatting-dprint.md), [Spell Checking (CSpell)](spell-checking-cspell.md), and [MarkdownLint](linting-markdownlint.md) — the three tools the linters workflow runs.
- [Testing and Code Coverage](testing-and-code-coverage.md) — the commands the tests workflow runs, in more detail.
- [Dependency Management](../conventions/dependency-management.md) — how the pinned SDK, tool, and package versions these workflows use are kept exact.
