# Commit Messages

This article covers how commit messages are written in this repository. It applies to everyone, human and AI assistant alike.

## The 50/72 Rule

Commit messages follow the conventional 50/72 rule:

- The subject line is at most 50 characters.
- A single blank line separates the subject from the body.
- Every body line is wrapped at most 72 characters — unlike the documentation, which is never manually wrapped (see [Markdown Style](markdown-style.md)).

The subject is a complete summary on its own. The body is optional — a small, self-explanatory change may be subject-only.

## Subject Line

The subject is **title case** (see [Markdown Style](markdown-style.md)) and **past tense**, describing what the commit did: `Added a GitHub Actions Workflow`, `Created a New Logo`, `Scaffolded the New Project`. As in a Markdown heading, the natural casing of code identifiers and brand names is preserved. It does not end with a period.

## Body

When present, the body explains what changed and why — the diff already shows what changed, so the *why* is what earns its place. It is written as prose paragraphs, bulleted or numbered lists, or a mix of both, freely intermixed (an opening paragraph followed by a list, followed by another paragraph, is fine). Whichever form it takes, it follows the same punctuation rule as the rest of the documentation: periods, not semicolons (see [Markdown Style](markdown-style.md)).

## Disclosing AI Involvement

Every commit message states whether AI was involved in producing it and, if it was, what exactly the AI did — this project is developed openly with AI assistance, and the commit history is where that involvement is tracked (see the "Use of AI" section of the [root README](../../../README.md)). A commit written entirely by hand carries no such note. A commit where AI was involved:

- Says in the body what the AI actually did (wrote the documentation, generated a design asset, tracked down a bug, and so on).
- Carries a trailer at the end of the body:

  ```text
  Co-Authored-By: Claude <noreply@anthropic.com>
  ```

## Example

```text
Added a GitHub Actions Workflow

A new GitHub Actions Workflow was added, which runs the linters and
the code formatter on every push to any branch.

The workflow was created using Claude Code.

Co-Authored-By: Claude <noreply@anthropic.com>
```

## Related

- The punctuation and title-case rules these messages follow: [Markdown Style](markdown-style.md).
- The full policy on AI-assisted work: the "Use of AI" section of the [root README](../../../README.md), and [Contributing](../contributing.md).
