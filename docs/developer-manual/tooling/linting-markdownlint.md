# Markdown Linting (MarkdownLint)

This article covers Markdown linting.

Markdown (including these docs) is linted by [markdownlint-cli2](https://github.com/DavidAnson/markdownlint-cli2), configured in [`.markdownlint.yml`](../../../tests/linters/.markdownlint.yml).

## Configuration

The config keeps the default rules but disables line-length (prose is not hard-wrapped) and allows the `kbd` inline-HTML element. Because line-length is off, do not manually wrap prose lines.

## Running It

After [installing the pinned Node.js dependencies](developer-setup.md) with `npm ci`:

```shell
npx markdownlint-cli2 --config tests/linters/.markdownlint.yml "**/*.md" "#**/build/**" "#**/node_modules/**" "#**/.venv/**"
```

`npx` resolves this to the exact version pinned in [`package.json`](../../../package.json) and locked in [`package-lock.json`](../../../package-lock.json), the same version [Continuous Integration](continuous-integration.md) installs. The trailing `#`-prefixed globs are `markdownlint-cli2`'s negated-glob syntax (it has no `--ignore` flag) and exclude build output, `node_modules`, and the Python `.venv` used to build the documentation website — unlike dprint and CSpell, `markdownlint-cli2` does not ignore these by default.

## Writing Docs That Pass

When writing an article, keep to standard Markdown: one top-level heading, blank lines around headings, lists, and fenced code blocks, and a language on every code fence. Wrap element or tag names in backticks (e.g. `` `dprint.json` ``) so they are code spans rather than inline HTML.

## Related

- Stylistic conventions that go beyond linting: [Markdown Style](../conventions/markdown-style.md).
- The pinned version used in CI: [Continuous Integration](continuous-integration.md).
