"""MkDocs "hooks" module (see properdocs.yml) that appends an "API Reference" entry to the ordered list on the documentation site's own overview
page ("docs/README.md").

"docs/api" is never committed - it only exists once "generate-api-docs.py" has run as part of a build (see
docs/developer-manual/tooling/documentation-website.md) - so a hand-written link to it in "docs/README.md" would 404 on GitHub, which only ever
shows the committed source. This hook instead appends the link at build time, after whichever ordered list item on the overview page is the last
one that starts with a link, using that item's own number plus one, so the numbering stays correct regardless of how many items the list already
has.
"""

import pathlib
import re
from typing import Any

# Matches one top-level ordered list item that starts with a Markdown link, capturing its number so the appended item can continue the same
# sequence; "re.MULTILINE" anchors "^" to the start of each line rather than the start of the whole page.
ordered_list_item_pattern = re.compile(r'^(\d+)\. \[.*$', re.MULTILINE)


def on_page_markdown(markdown: str, page: Any, config: Any, **kwargs: Any) -> str:
    """Appends the "API Reference" list item to "docs/README.md"'s ordered list, immediately after its last link-led item.

    Args:
        markdown: The page's raw Markdown source, before it is converted to HTML.
        page: The ProperDocs page "markdown" was read for. Only the site's root overview page ("docs/README.md", "file.src_uri" equal to
            "README.md") is modified; every other page is returned unchanged.
        config: The ProperDocs configuration for the current build; unused, but required by the hook's signature.
        kwargs: Additional arguments the "on_page_markdown" event may be called with; unused, but required by the hook's signature.

    Returns:
        "markdown" with the "API Reference" item appended after the overview page's last link-led ordered list item, or unchanged for every
        other page, or if the overview page has no such list item to append after.
    """

    if pathlib.PurePosixPath(page.file.src_uri) != pathlib.PurePosixPath('README.md'):
        return markdown

    list_item_matches = list(ordered_list_item_pattern.finditer(markdown))
    if not list_item_matches:
        return markdown

    last_list_item_match = list_item_matches[-1]
    next_item_number = int(last_list_item_match.group(1)) + 1
    insertion_point = last_list_item_match.end()
    api_reference_item = f'\n{next_item_number}. [API Reference](api/index.md) — generated from the C# XML documentation comments at build time.'

    return markdown[:insertion_point] + api_reference_item + markdown[insertion_point:]
