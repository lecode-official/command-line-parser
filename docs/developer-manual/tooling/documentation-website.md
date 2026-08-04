# Documentation Website

This article covers the [ProperDocs](https://properdocs.org/) site, themed with [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/), that publishes everything under [`docs/`](../../../docs), including the C# API reference that is generated from source at build time.

## Why ProperDocs, Not MkDocs

This site was originally built on [MkDocs](https://www.mkdocs.org/), but MkDocs itself has gone unmaintained. [ProperDocs](https://properdocs.org/) is a fork by MkDocs' own last active maintainer, positioned as an exact drop-in continuation of the MkDocs 1.x line rather than a break from it: it reads the same configuration format, and it registers both `mkdocs.*` and `properdocs.*` plugin and theme entry points, so [mkdocs-material](https://squidfunk.github.io/mkdocs-material/), [mkdocs-gen-files](https://github.com/oprypin/mkdocs-gen-files), and [mkdocs-literate-nav](https://oprypin.github.io/mkdocs-literate-nav/) — all still published under their original MkDocs-branded package names — work with it unmodified. Only the site-generator engine itself changed; the theme and the plugin are untouched.

## Configuration

[`properdocs.yml`](../../../properdocs.yml) lives at the repository root and points `docs_dir` at the existing [`docs/`](../../../docs) tree, so every hand-written article in [User Manual](../../user-manual/README.md) and this Developer Manual is part of the site without having been moved or renamed. `site_dir` is set to `build/documentation-website`, alongside the redirected `bin`/`obj` output of the three `.NET` projects (see [Architecture](../architecture.md)), rather than ProperDocs' own default of `site/`. Three plugins are enabled:

- **search** — the built-in full-text search, enabled by default.
- **gen-files** ([mkdocs-gen-files](https://github.com/oprypin/mkdocs-gen-files)) — runs [`docs/build/generate-api-docs.py`](../../build/generate-api-docs.py) at the start of every build to produce the `api` section (see below).
- **literate-nav** ([mkdocs-literate-nav](https://oprypin.github.io/mkdocs-literate-nav/)) — lets the `api` entry in `nav` (see below) point at the `api/` directory instead of a single file, so its own navigation comes from a generated `api/SUMMARY.md` rather than being hand-listed.

`markdown_extensions` in [`properdocs.yml`](../../../properdocs.yml) adds [mdx_truly_sane_lists](https://github.com/radude/mdx_truly_sane_lists), configured with `nested_indent: 3`. Without it, Python-Markdown (the engine underneath ProperDocs) only recognizes a nested list item when it is indented by a fixed 4 spaces, regardless of the parent marker's width — the various `README.md` files' tables of contents nest ordered sub-lists under `1.`-style markers using the natural, minimal 3-space indent, which Python-Markdown's default renders as a second top-level list instead. `nested_indent: 3` makes the required indent relative to the parent marker's width instead, so those tables of contents nest correctly without reformatting any Markdown file.

## Light and Dark Mode

`theme.palette` in [`properdocs.yml`](../../../properdocs.yml) lists two [mkdocs-material palettes](https://squidfunk.github.io/mkdocs-material/setup/changing-the-colors/#color-palette-toggle), one for `scheme: default` (light) and one for `scheme: slate` (dark), each tied to a `media: "(prefers-color-scheme: ...)"` query. A visitor's browser reports which scheme their OS is set to, and mkdocs-material's built-in script reads that to select the matching palette on load — so the site opens in light or dark mode automatically, with no configuration needed on the visitor's part. Each palette also carries a `toggle` (a sun icon on the light palette, a moon icon on the dark one) that lets a visitor override the OS choice for the rest of the session; mkdocs-material stores that override in the browser and remembers it on the next visit.

### Making a Light/Dark Image Pair Work on Both GitHub and This Site

[`docs/README.md`](../../README.md) and the root [`README.md`](../../../README.md) both show the [`CLI.NET Core` logo](../logo-design.md) as a pair of images — one for light mode, one for dark — using GitHub's own convention for this: an `<img>` whose `src` ends in the literal suffix `#gh-dark-mode-only` or `#gh-light-mode-only`, which GitHub's renderer detects and uses to hide the non-matching image. Since `docs/README.md` is also a page on this site, that same pair needs to switch with mkdocs-material's own light/dark scheme too. mkdocs-material has its own, near-identical built-in convention for this — `#only-dark` / `#only-light` — but it is a **different** suffix, and since the trick on both sides is a literal string match on the end of the `src` attribute, one `<img>` cannot satisfy two different literal suffixes at once. Rather than write out each image twice with a different suffix per platform (which would show two copies of each image on whichever platform does not recognize the other's suffix), [`docs/assets/styles/gh-mode-only-images.css`](../../assets/styles/gh-mode-only-images.css) — wired in via `extra_css` in [`properdocs.yml`](../../../properdocs.yml) — teaches mkdocs-material to also recognize GitHub's own suffixes, the same way mkdocs-material's own CSS recognizes its `#only-dark` / `#only-light` suffixes: an attribute selector matching `img[src$="#gh-dark-mode-only"]` (and the light equivalent), scoped to the `data-md-color-scheme` attribute mkdocs-material's light/dark toggle sets on the page. This keeps every Markdown image tag identical on both platforms — no `#only-dark` / `#only-light` suffix is used anywhere in this repository, only GitHub's.

### The Site Header Logo

`theme.logo` in [`properdocs.yml`](../../../properdocs.yml) sets the small badge mkdocs-material renders in the site header and in the mobile navigation drawer, to `assets/images/logo-light.png`. Unlike the light/dark image pair above, `theme.logo` is mkdocs-material's single, built-in header logo setting — it takes exactly one file, with no light/dark option of its own, so the value here is only the fallback. [`docs/assets/styles/site-logo.css`](../../assets/styles/site-logo.css) — also wired in via `extra_css` in [`properdocs.yml`](../../../properdocs.yml) — swaps in `assets/images/logo-dark.png` for the dark palette, using the same `data-md-color-scheme="slate"` attribute selector as [`gh-mode-only-images.css`](../../assets/styles/gh-mode-only-images.css) above, but applied to the CSS `content` property instead of `display`: since `theme.logo` renders as one `<img>`, not a pair, there is nothing to hide, only a single rendered image to replace. `logo-light.png` and `logo-dark.png` are the 64×64 primary glass badge from the [design guide](../logo-design.md) (`badge-glass-light.svg` / `-dark.svg`), baked to PNG the same way the light/dark image pair above is. The design guide's flat badge exists for cases where filter effects get stripped or the badge renders too small to read them (its favicon use, above, is exactly that case) — neither applies here, since this is a plain PNG `<img>` on this site's own header, not an SVG sanitized by GitHub, and 64×64 is large enough for the glass effect to read.

### Static Assets

Images and other static files the documentation itself needs (as opposed to files a plugin generates, like `docs/api`) live under [`docs/assets/`](../../assets) — for example [`docs/assets/images/`](../../assets/images) holds the baked PNGs of the logo used above, and [`docs/assets/styles/`](../../assets/styles) holds the CSS files mentioned above. Anything under `docs_dir` that is not excluded via `exclude_docs` is copied into the built site as-is, at the same relative path, so `docs/assets/images/header-logo-dark.png` becomes `assets/images/header-logo-dark.png` on the built site — which is also why `docs/README.md`'s copy of the logo pair, and `properdocs.yml`'s `theme.logo`, both use that same `assets/images/...` path rather than reaching up into [`design/`](../../../design), which is not published. The [design guide](../logo-design.md) keeps the vector masters that these PNGs were baked from; only the baked PNGs themselves live under `docs/assets/images/`.

Two subdirectories of `docs/assets/` are excluded from the built site itself, via `exclude_docs` in [`properdocs.yml`](../../../properdocs.yml):

- [`docs/build/`](../../build) — the four Python scripts described below (`generate-api-docs.py`, `copy-favicon-to-site-root.py`, `rewrite-repo-links.py`, `inject-api-reference-link.py`), which run at build time rather than being published as pages or static files.
- [`docs/assets/templates/`](../../assets/templates) — the one theme template described below (`main.html`), read by mkdocs-material itself rather than being published.

## Links Outside `docs/`

An article under `docs/` is free to link to a file anywhere else in the repository — source code, [`README.md`](../../../README.md), [`CLAUDE.md`](../../../CLAUDE.md), a workflow under [`.github/workflows/`](../../../.github/workflows/), and so on — using an ordinary relative Markdown link, the same as it would link to another article. That link renders correctly on GitHub, which resolves it against the file's real position in the repository, but the built site has no such files: `docs_dir` is the only part of the repository ProperDocs ever copies into `site_dir`, so the exact same link would 404 there.

[`docs/build/rewrite-repo-links.py`](../../build/rewrite-repo-links.py) closes that gap. Wired in via `hooks:` in [`properdocs.yml`](../../../properdocs.yml) alongside `copy-favicon-to-site-root.py`, it defines an `on_page_markdown` function — a plain [MkDocs `hooks`](https://www.mkdocs.org/user-guide/configuration/#hooks) event, like `on_post_build` above, so no plugin package is needed for it either — that runs on every page's raw Markdown before it is converted to HTML. For every link on the page, it resolves the link's target against the page's own file on disk and checks whether the result still falls inside `docs_dir`:

- **Inside `docs_dir`** — left untouched. ProperDocs already resolves and rewrites this kind of link correctly on its own.
- **Outside `docs_dir`, inside the repository** — rewritten into an absolute link at `repo_url`, pointing at the `main` branch (the one the site itself is always built and deployed from, see [Continuous Integration](continuous-integration.md)) — `tree` for a link to a directory, `blob` for a link to a file, matching GitHub's own URL scheme. A fragment on the original link (for example `CLAUDE.md#golden-rules`) is preserved on the rewritten one.
- **Outside the repository entirely** — left untouched. A link like this is already broken on GitHub, and rewriting it further is not this hook's concern.

The generated [`api`](#the-api-reference-docsapi) section is skipped entirely: none of its cross-linking ever points outside `docs_dir`, and its pages exist only in memory during the build (see below), not at a real path this hook could resolve links against.

## Navigation Is One Hand-Maintained `nav` List, Plus One Generated Section

The site navigation is the single `nav` list in [`properdocs.yml`](../../../properdocs.yml) — there is no `.pages` file anywhere under `docs/`, and no plugin builds the *hand-written* part of the site's navigation from the directory tree. Adding, removing, moving, or reordering an article means editing that one list; nothing else in the repository has to change.

The one exception is the `api` entry, `API Reference: api/`. Its file list is not known until a build actually runs xmldoc2md, so it cannot be hand-listed the way the rest of the site is. Instead, that entry's trailing slash tells **literate-nav** to look for a nav file inside `api/` — and [`generate-api-docs.py`](../../build/generate-api-docs.py) writes that nav file itself (see below), the same way it writes the pages it lists. No `.pages` file is involved: one Python script controls both the content and the navigation of the one section that cannot be hand-listed, and every other section stays exactly as hand-listed as before.

## The API Reference (`docs/api`)

The `api` section is generated entirely at build time and is **never committed** — there is no `docs/api` directory in the repository, and none is expected to appear; no `.gitignore` entry exists for it because nothing ever writes it to disk in the first place. Every `properdocs build` or `properdocs serve` run regenerates it from whatever the C# XML documentation comments currently say.

[`docs/build/generate-api-docs.py`](../../build/generate-api-docs.py) does this in three steps:

1. **Builds the project.** It runs `dotnet build` on the sample app (which transitively builds `clinet-core`), so that the framework's XML documentation file exists. The sample app, not `clinet-core` directly, is what gets built and read: a class library's own build output does not include its NuGet dependencies, and the next step needs every dependency of the framework assembly to be loadable, which only the sample app's build output provides. If `GenerateDocumentationFile` were ever disabled on [`clinet-core`'s `.csproj`](../../../source/clinet-core/CLI.NET%20Core.csproj), this step fails loudly rather than silently producing an empty API reference.
2. **Generates Markdown.** It shells out to [xmldoc2md](https://github.com/charlesdevandiere/xmldoc2md), pinned in [`.config/dotnet-tools.json`](../../../.config/dotnet-tools.json) like the other [local .NET tools](../conventions/dependency-management.md#local-net-tools), writing its output to a temporary directory — never into `docs/`.
3. **Injects the pages and their navigation.** It reads every Markdown file xmldoc2md wrote and re-emits it through `mkdocs_gen_files.open()`, which hands the content straight to the site generator's in-memory build. While doing so, it also records each page in an [`mkdocs_gen_files.Nav`](https://oprypin.github.io/mkdocs-gen-files/extras.html), and once every page has been injected, writes that `Nav`'s `build_literate_nav()` output to `api/SUMMARY.md` — the file **literate-nav** looks for by default. The temporary directory is discarded once the build has consumed it.

Every page's entry in that `Nav` is built from the page's own content, not from the all-lowercase file and folder names `--structure tree` derived from the C# namespaces and type names — xmldoc2md has no option to change those (there is no such flag; `dotnet tool run xmldoc2md -- --help` was checked), and short of forking it there is no way to make it emit anything else:

- A type's page is grouped under its real, properly cased namespace segments (for example `CliNetCore` and `Application`, not `clinetcore` and `application`), parsed from the `Namespace: CliNetCore.Application` line xmldoc2md puts on every such page.
- A type's own navigation label is its real name (for example `CliCommandLineArguments`), not the all-lowercase file name xmldoc2md derived it from, parsed from the page's H1 heading.
- The one page with neither a `Namespace:` line nor a fitting H1 to reuse — the root index page xmldoc2md writes for the whole assembly, whose H1 is the assembly's own name (`CLI.NET Core`) — is labeled `Index` instead, since reusing that H1 as a navigation label next to the type names it links to would be confusing.

`api/SUMMARY.md` keeps the plugin's default name rather than being renamed to `README.md` to match every other section's overview page: `nav_file` is a global plugin setting, not one scoped per directory, and it makes literate-nav check for that filename at `docs_dir`'s own root before it even reads `nav` in [`properdocs.yml`](../../../properdocs.yml) — since a hand-written `docs/README.md` already exists there, renaming `nav_file` to `README.md` makes literate-nav try to parse that hand-written prose page as a literate nav list instead of using the hand-maintained `nav` list at all.

The script itself lives inside `docs_dir`'s `build/` subdirectory (see [Static Assets](#static-assets)), so it is excluded via `exclude_docs` in [`properdocs.yml`](../../../properdocs.yml) — without that, the site generator would treat it like any other non-Markdown file under `docs/` and copy it verbatim into the built site.

### Why xmldoc2md

xmldoc2md and [DefaultDocumentation](https://github.com/Doraku/DefaultDocumentation) were both tried against this codebase before choosing. DefaultDocumentation is more configurable, but its defaults document `private` members and name one file per member with literal parentheses and commas in the filename (for example `CliNetCore.Application.CliApplicationBuilder.CreateBuilder(string[]).md`) — both unsuitable for a public API reference and for clean, predictable URLs, and both would need explicit configuration to fix. xmldoc2md's defaults already do the right thing for this project: it documents `protected` and `public` members only, produces one clean file per type organized into a namespace tree (`--structure tree`), and has a `--platform github-pages` preset that shapes links and front matter for exactly this kind of static site. It needed no extra configuration beyond the two flags used above.

### Why Members With `<inheritdoc/>` Need Another Package

A member documented with `<inheritdoc/>` (see [C# Style](../conventions/csharp-style.md)) — for example every property `CliApplicationBuilder` implements from `IHostApplicationBuilder` — showed up in `docs/api` with no description at all, just its signature. The cause is not xmldoc2md: the C# compiler never expands `<inheritdoc/>` into real text in the generated XML documentation file, it is left in verbatim, so any tool that reads that file — xmldoc2md, or a consumer's editor tooltip for the compiled package — sees an empty member. [`clinet-core`'s `.csproj`](../../../source/clinet-core/CLI.NET%20Core.csproj) references [SauceControl.InheritDoc](https://github.com/saucecontrol/InheritDoc), pinned like every other dependency (see [Dependency Management](../conventions/dependency-management.md)), which rewrites `<inheritdoc/>` with the real, inherited text as a build step — for every non-Debug build, which is why step 1 above builds in `Release` rather than the default `Debug` configuration.

## Linking to the API Reference from the Overview Page

[`docs/README.md`](../../README.md) — the site's own overview page — lists the [Developer Manual](../README.md) and [User Manual](../../user-manual/README.md) as an ordered list, and that list also needs an entry pointing at `docs/api`. A hand-written link to it cannot work: `docs/api` is generated entirely at build time (see above) and never committed, so a relative link to it in `docs/README.md`'s source would 404 when GitHub renders that file directly, the same problem [Links Outside `docs/`](#links-outside-docs) solves for links leaving `docs_dir` entirely — except here there is no repository file to redirect to instead, since `docs/api` does not exist outside a build.

[`docs/build/inject-api-reference-link.py`](../../build/inject-api-reference-link.py) closes this gap the same way [`rewrite-repo-links.py`](../../build/rewrite-repo-links.py) does, with another `on_page_markdown` hook wired in via `hooks:` in [`properdocs.yml`](../../../properdocs.yml). Scoped to the site's root overview page only (`page.file.src_uri` equal to `README.md`), it finds the last ordered list item that starts with a Markdown link, reads that item's own number, and appends a new `[API Reference](api/index.md)` item numbered one higher — so `docs/README.md`'s source never lists `docs/api` at all, and the list still numbers correctly regardless of how many hand-written items precede it. `api/index.md` is a safe target because [`generate-api-docs.py`](../../build/generate-api-docs.py) always writes it (it is the root page xmldoc2md generates for the whole assembly, labeled `Index` in the nav — see above), and because it resolves inside `docs_dir`, so `rewrite-repo-links.py` leaves it untouched regardless of which of the two hooks runs first.

## No Cross-Linking to `docs/api` Yet

Type names mentioned in [User Manual](../../user-manual/README.md) or this Developer Manual's prose and code blocks stay plain text or plain code — they do not link to their `docs/api` page. Adding that (for example with [mkdocs-autorefs](https://github.com/mkdocstrings/autorefs)) is deliberately out of scope for now. The overview page's own `API Reference` list entry (see above) is the one exception, since it links to the section as a whole rather than to an individual type.

## Favicon

The favicon is baked from the [design guide](../logo-design.md)'s flat badge (`badge-flat-dark.svg` / `-light.svg`, chosen over the glass badge because a favicon is rendered too small for the glass effect to read) into three files under [`docs/assets/favicon/`](../../assets/favicon):

- `favicon-light.svg` and `favicon-dark.svg` — the two SVGs themselves, used as described below.
- `favicon.ico` — a multi-resolution (16×16, 32×32, 48×48) fallback baked from the light SVG, for browsers that ignore the two files above.

A favicon has no equivalent of the `#gh-dark-mode-only` / `#gh-light-mode-only` trick described above, because it is not an `<img>` in the page — it is one or more `<link rel="icon">` elements in the page's `<head>`, and which one a browser uses is up to the browser, not to this site's CSS. `theme.favicon: assets/favicon/favicon.ico` in [`properdocs.yml`](../../../properdocs.yml) is mkdocs-material's single, built-in favicon setting, and is only ever a single file — it has no light/dark option. The light/dark pair is added on top of it by [`docs/assets/templates/main.html`](../../assets/templates/main.html), a `theme.custom_dir` template that extends mkdocs-material's own `main.html` and fills only its `extrahead` block — the block Material's own documentation reserves for exactly this kind of addition, so this override tracks Material's own head markup instead of duplicating it — with two extra `<link rel="icon" media="(prefers-color-scheme: ...)">` elements, one per SVG. Browser support for `media` on a favicon `<link>` is inconsistent, so `favicon.ico` stays in place as the fallback for browsers that ignore both. `theme.custom_dir` is the one path in [`properdocs.yml`](../../../properdocs.yml) that is relative to the repository root rather than to `docs_dir`, which is why `assets/templates/` still needs its own `exclude_docs` entry (see [Static Assets](#static-assets)) even though it sits under `docs/assets/` alongside every other static asset.

Browsers and crawlers also request `/favicon.ico` at the site's root directly, regardless of any `<link>` element on the page. Since `docs/assets/favicon/` keeps the favicon grouped with the rest of the [static assets](#static-assets) rather than loose at `docs_dir`'s own root, it builds to `assets/favicon/favicon.ico`, not `favicon.ico`, on the site — [`docs/build/copy-favicon-to-site-root.py`](../../build/copy-favicon-to-site-root.py) closes that gap. It is a plain MkDocs [`hooks`](https://www.mkdocs.org/user-guide/configuration/#hooks) module (a feature of MkDocs core itself, not a plugin, so no plugin package or entry point is needed for it), wired in via `hooks:` in [`properdocs.yml`](../../../properdocs.yml), defining a single `on_post_build` function that runs after every build and copies the already-built `assets/favicon/favicon.ico` to the site's own root. Like [`generate-api-docs.py`](../../build/generate-api-docs.py), it lives inside `docs_dir`'s `build/` subdirectory and is excluded via `exclude_docs` so the site generator does not also copy the script itself into the built site as a static asset.

## Running It Locally

Install the pinned Python dependencies from [`requirements.txt`](../../../requirements.txt) (see [Developer Setup](developer-setup.md)):

```shell
python -m pip install --requirement requirements.txt
```

Then, from the repository root:

```shell
properdocs serve
```

serves the site at `http://127.0.0.1:8000` with live reload for anything under `docs/` (including `docs/build/generate-api-docs.py`) and `properdocs.yml`. To produce the static site once, into `build/documentation-website/` (already covered by [`.gitignore`](../../../.gitignore) as build output, alongside the `.NET` projects' own redirected build output — see [Architecture](../architecture.md)):

```shell
properdocs build
```

Both commands regenerate `docs/api` from scratch every time, as described above — there is nothing to clean up afterwards.

## Related

- [Developer Setup](developer-setup.md) — installing Python and the pinned ProperDocs dependencies.
- [Dependency Management](../conventions/dependency-management.md) — how `requirements.txt` and the local .NET tool pin their exact versions.
- [Continuous Integration](continuous-integration.md) — the workflow that builds this site and deploys it to GitHub Pages on every push to `main`.
- [Design Guide](../logo-design.md) — the vector masters the logo and favicon are baked from, and how to use them on GitHub.
