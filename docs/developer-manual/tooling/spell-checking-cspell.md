# Spell Checking (CSpell)

This article covers spell checking of code, comments, and documentation.

[CSpell](https://cspell.org/) is configured in [`.cspell.json`](../../../tests/linters/.cspell.json) and checks English (`en-US`), alongside German, Spanish, and French dictionaries for names and terms that show up in comments or documentation.

## Dictionaries

Beyond the base English dictionary, the configuration adds dictionaries of common misspellings, of the keywords used by the languages in this project (`bash`, `docker`, `git`, `csharp`), and a handful of general-purpose technical dictionaries (companies, computing acronyms, file types, public licenses, software terms, and so on). The Visual Studio Code extension (see [Visual Studio Code Integration](vscode-integration.md)) ships the same English dictionaries; additional dictionaries can be installed as further Visual Studio Code extensions if needed.

## Ignored Paths and Custom Words

Build output (`bin` and `obj`), SVG files, and OS metadata files (`.DS_Store`, `Thumbs.db`) are ignored. Project-specific terms — proper names, tool names, and anything else not in the standard dictionaries — live in the `words` list in the config.

## Running It and Handling Findings

After [installing the pinned Node.js dependencies](developer-setup.md) with `npm ci`:

```shell
npx cspell lint --config tests/linters/.cspell.json --no-progress "**/*"
```

`npx` resolves this to the exact version pinned in [`package.json`](../../../package.json) and locked in [`package-lock.json`](../../../package-lock.json), the same version [Continuous Integration](continuous-integration.md) installs, so a local run agrees with what CI reports. When a legitimate technical term or proper noun is flagged, add it to the `words` list in [`.cspell.json`](../../../tests/linters/.cspell.json) rather than rewording the text.

## Related

- The pinned version used in CI: [Continuous Integration](continuous-integration.md).
