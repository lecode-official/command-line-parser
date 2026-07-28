# Continuous Integration

This article covers the GitHub Actions workflow in [`.github/workflows/linters.yml`](../../../.github/workflows/linters.yml).

## What It Runs

The workflow runs on every push, to any branch. A single job, on the latest Ubuntu runner, checks out the repository, installs Node.js 24, installs pinned versions of the three tools this project relies on for formatting and linting, and then runs each of them in turn:

- `dprint check "**/*"` — verifies that every file dprint covers is already formatted (see [dprint](formatting-dprint.md)); unlike `dprint fmt`, it never modifies a file.
- `cspell lint --config tests/linters/.cspell.json --no-progress "**/*"` — spell-checks the whole repository (see [Spell Checking (CSpell)](spell-checking-cspell.md)).
- `markdownlint-cli2 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"` — lints every Markdown file, excluding build output (see [MarkdownLint](linting-markdownlint.md)).

The spell-checking and Markdown-linting steps run even if an earlier step failed (`if: ${{ !cancelled() }}`), so a single failing tool does not hide findings from the others — the workflow always reports everything it can in one run.

## Keeping Versions in Sync

The tool versions installed in the workflow (`dprint`, `cspell`, `markdownlint-cli2`) are pinned exactly. When running these tools locally (see the "Running It" section of each tool's article), use the same pinned versions so that a local pass reliably predicts a green CI run.

There is currently no separate build or test job — the framework's own test suite (see [`tests/unit-tests`](../../../tests/unit-tests/)) is not yet wired into this workflow.

## Related

- [dprint](formatting-dprint.md), [Spell Checking (CSpell)](spell-checking-cspell.md), and [MarkdownLint](linting-markdownlint.md) — the three tools this workflow runs.
