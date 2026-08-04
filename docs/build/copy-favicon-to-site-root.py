"""MkDocs "hooks" module (see properdocs.yml) that copies the built favicon.ico to the site's root once a ProperDocs build has finished.

Browsers and crawlers request "/favicon.ico" at the domain root as a fallback, independent of whatever "<link rel="icon">" tags a page's "<head>"
carries (see docs/developer-manual/tooling/documentation-website.md). "theme.favicon" in properdocs.yml only ever writes the icon to the path
given there - "docs/assets/favicon/favicon.ico", kept there so the favicon sits alongside the site's other static assets under "docs/assets/" -
never to the site root, so this hook copies the already-built file there after the rest of the site has been written.
"""

import pathlib
import shutil
from typing import Any


def on_post_build(config: Any, **kwargs: Any) -> None:
    """Copies "assets/favicon/favicon.ico" from the built site into the site's own root directory.

    Args:
        config: The ProperDocs configuration for the build that just finished, whose "site_dir" is where the site was written.
        kwargs: Additional arguments the "on_post_build" event may be called with; unused, but required by the hook's signature.
    """

    site_directory = pathlib.Path(config['site_dir'])
    built_favicon_path = site_directory / 'assets' / 'favicon' / 'favicon.ico'
    shutil.copyfile(built_favicon_path, site_directory / 'favicon.ico')
