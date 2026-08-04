"""MkDocs "hooks" module (see properdocs.yml) that rewrites Markdown links pointing outside "docs_dir" into absolute GitHub URLs.

Every hand-written article under "docs/" is free to link to files elsewhere in the repository - source code, "README.md", "CLAUDE.md", the
".github/workflows/" directory, and so on (see docs/developer-manual/tooling/documentation-website.md) - because those links already render
correctly on GitHub, which resolves a relative link against the file's real position in the repository. The built site has no such files:
"docs_dir" is the only part of the repository ProperDocs ever copies into "site/", so a relative link that climbs out of "docs/" would 404 there
even though the exact same link works on GitHub. This hook rewrites every such link, at build time, into an absolute "repo_url" link to the same
file on GitHub instead - a link that already resolves inside "docs_dir" is left untouched, since ProperDocs already resolves and rewrites those
correctly on its own.
"""

import pathlib
import re
import urllib.parse
from typing import Any

# The branch the built site is always deployed from (see docs/developer-manual/tooling/continuous-integration.md - the docs site is only ever
# built and published on a push to "main"), so a rewritten link always points at the same revision the live site itself was built from.
branch_name = 'main'

# The repository root, resolved relative to this script rather than the current working directory, the same way generate-api-docs.py does it; this
# script lives inside "docs_dir"'s "build" subdirectory (see properdocs.yml's "exclude_docs"), two levels below the repository root.
repository_root: pathlib.Path = pathlib.Path(__file__).resolve().parent.parent.parent

# Matches one Markdown link or image, capturing its "[label]" (including the leading "!" of an image), its raw target, and its optional
# "title" so both can be put back around a rewritten target unchanged.
markdown_link_pattern = re.compile(r'(!?\[[^\]]*\])\(([^)\s]+)(\s+"[^"]*")?\)')


def on_page_markdown(markdown: str, page: Any, config: Any, **kwargs: Any) -> str:
    """Rewrites every Markdown link in "markdown" that resolves outside "docs_dir" into an absolute GitHub URL.

    Args:
        markdown: The page's raw Markdown source, before it is converted to HTML.
        page: The ProperDocs page "markdown" was read for. Its "file.src_uri" identifies the generated "api/" section (see
            docs/developer-manual/tooling/documentation-website.md), which is skipped, since its links only ever point at other generated "api/"
            pages, never outside "docs_dir".
        config: The ProperDocs configuration for the current build, whose "docs_dir" bounds which resolved links count as "outside" and whose
            "repo_url" is the GitHub repository a rewritten link points into.
        kwargs: Additional arguments the "on_page_markdown" event may be called with; unused, but required by the hook's signature.

    Returns:
        "markdown" with every outside-pointing link rewritten to an absolute GitHub URL, or unchanged for a page in the generated "api/" section.
    """

    if pathlib.PurePosixPath(page.file.src_uri).parts[0] == 'api':
        return markdown

    source_directory = pathlib.Path(page.file.abs_src_path).parent
    docs_directory = pathlib.Path(config['docs_dir'])
    repo_url = config['repo_url']

    def rewrite(match: re.Match[str]) -> str:
        label, target, title = match.group(1), match.group(2), match.group(3) or ''
        rewritten_target = rewrite_target(target, source_directory, docs_directory, repo_url)
        return f'{label}({rewritten_target}{title})'

    return markdown_link_pattern.sub(rewrite, markdown)


def rewrite_target(target: str, source_directory: pathlib.Path, docs_directory: pathlib.Path, repo_url: str) -> str:
    """Rewrites a single link target into an absolute GitHub URL, if and only if it resolves outside "docs_directory".

    Args:
        target: One Markdown link's raw target, exactly as written - for example "../../CLAUDE.md#golden-rules".
        source_directory: The directory of the page the link was found on, which "target" is resolved against.
        docs_directory: The site's "docs_dir", the boundary between an internal link (left untouched) and one this hook rewrites.
        repo_url: The GitHub repository a rewritten link points into.

    Returns:
        "target" unchanged if it is not a relative filesystem path, already resolves inside "docs_directory", or resolves outside the repository
        entirely (a broken link this hook cannot make sense of); otherwise, the equivalent absolute GitHub URL, with the original fragment (if
        any) preserved.
    """

    path_part, fragment_separator, fragment = target.partition('#')
    if not path_part or re.match(r'^[a-zA-Z][a-zA-Z0-9+.-]*:', path_part):
        # Empty (a pure "#fragment" link) or already absolute (an URL scheme like "https:" or "mailto:") - nothing to rewrite.
        return target

    resolved_path = (source_directory / urllib.parse.unquote(path_part)).resolve()

    try:
        resolved_path.relative_to(docs_directory)
        return target
    except ValueError:
        pass

    try:
        relative_to_repository_root = resolved_path.relative_to(repository_root)
    except ValueError:
        # Outside the repository entirely - a broken link, which is not this hook's concern to fix.
        return target

    reference = 'tree' if resolved_path.is_dir() else 'blob'
    github_url = f'{repo_url}/{reference}/{branch_name}/{urllib.parse.quote(relative_to_repository_root.as_posix())}'
    return f'{github_url}{fragment_separator}{fragment}' if fragment_separator else github_url
