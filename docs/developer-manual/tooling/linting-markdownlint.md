# MarkdownLint

This article covers Markdown linting.

Markdown (including these docs) is linted by [markdownlint-cli2](https://github.com/DavidAnson/markdownlint-cli2), configured in [`.markdownlint.yml`](../../../tests/linters/.markdownlint.yml).

## Configuration

The config keeps the default rules but disables line-length (prose is not hard-wrapped) and allows the `kbd` inline-HTML element. Because line-length is off, do not manually wrap prose lines.

## Running It

```shell
npx --yes markdownlint-cli2@0.23.0 --config tests/linters/.markdownlint.yml "**/*.md" "#**/bin/**" "#**/obj/**"
```

The version matches the one [Continuous Integration](continuous-integration.md) installs. The trailing `#`-prefixed globs are `markdownlint-cli2`'s negated-glob syntax (it has no `--ignore` flag) and exclude build output.

## Writing Docs That Pass

When writing an article, keep to standard Markdown: one top-level heading, blank lines around headings, lists, and fenced code blocks, and a language on every code fence. Wrap element or tag names in backticks (e.g. `` `dprint.json` ``) so they are code spans rather than inline HTML.

## Related

- Stylistic conventions that go beyond linting: [Markdown Style](../conventions/markdown-style.md).
- The pinned version used in CI: [Continuous Integration](continuous-integration.md).
