# Markdown Style

This article covers the conventions for Markdown files (this documentation, `README.md`, etc.). Markdown linting itself is handled by MarkdownLint (see [MarkdownLint](../tooling/linting-markdownlint.md)). This is about the conventions on top.

## Headings Are Title Case

Every heading — at every level, in every Markdown file — uses **title case**, not sentence case. Write `## Running the App`, not `## Running the app`.

The rules:

- Capitalize the first and last word, and every major word in between (nouns, verbs, adjectives, adverbs, pronouns, and subordinating conjunctions).
- Keep minor words lowercase unless they are first or last: articles (`a`, `an`, `the`), coordinating conjunctions (`and`, `but`, `or`, `nor`, `for`, `so`, `yet`), and short prepositions (`as`, `at`, `by`, `for`, `from`, `in`, `into`, `of`, `off`, `on`, `per`, `to`, `up`, `via`, `vs`, `with`).
- In a hyphenated compound, apply the same rules to each part: `Main-CLI Build`, `One-off-Lifecycle Events`.

## Preserve Exact Casing of Names and Code

Title case never overrides the real casing of a name. Leave these exactly as they are written:

- Code spans stay verbatim, including surrounding backticks: ``## The `bootstrap` Pattern``.
- Brand and tool names keep their own casing, including intentionally lowercase ones: `dprint`.
- Acronyms and proper technology names stay as-is: `CLI`, `macOS`, `i18n`.

When a name is intrinsically lowercase, it stays lowercase even as the first word: `## dprint Owns Formatting`.

## Punctuation

- **Prefer a period over a semicolon.** Where a semicolon joins two full sentences, split them into two sentences with a period instead. This applies to all prose — the documentation, `README.md`, etc. In-code comments are the one exception and follow their own rule (see [C# Style](csharp-style.md)).
- **Use em and en dashes sparingly.** They fit a genuine aside or an abrupt break, not as a default connector. When a comma, a period, or parentheses reads just as well, use that instead.

## No Horizontal Rules

Never use horizontal rules (`---`) to separate sections. Headings already provide the structure. A `---` at the top of a file is only ever front matter, never a divider.

## Related

- Markdown linting: [MarkdownLint](../tooling/linting-markdownlint.md).
- The other conventions: [C# Style](csharp-style.md).
