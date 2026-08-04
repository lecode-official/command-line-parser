"""Generates the "api" section of the documentation website at ProperDocs build time from CLI.NET Core's XML documentation comments.

Nothing this script produces ever touches the real "docs" directory: xmldoc2md writes its Markdown files to a temporary directory, which this
script reads and re-emits through mkdocs_gen_files.open() into the in-memory build, and the temporary directory is discarded once the build has
consumed it. See docs/developer-manual/tooling/documentation-website.md for the full explanation of this flow.
"""

import pathlib
import re
import subprocess
import tempfile

import mkdocs_gen_files

# The repository root, resolved relative to this script rather than the current working directory, so the script works regardless of where
# "properdocs build" or "properdocs serve" is invoked from; this script lives inside "docs_dir"'s "build" subdirectory (see properdocs.yml's
# "exclude_docs"), two levels below the repository root
repository_root: pathlib.Path = pathlib.Path(__file__).resolve().parent.parent.parent

# The project whose build output is fed to xmldoc2md; the sample app is used instead of "clinet-core" directly because a class library's own
# build output does not include its NuGet dependencies (they are only resolved, not copied), and xmldoc2md needs every dependency of the
# framework assembly loadable via reflection; building the sample app transitively builds "clinet-core" and copies the framework assembly, its
# XML documentation file, and the full dependency closure into one output folder
documented_project: pathlib.Path = repository_root / 'source' / 'sample-app' / 'CLI.NET Core Sample App.csproj'
build_configuration: str = 'Release'

# "build/sample-app/bin/", not "source/sample-app/bin/" - the repository root's "Directory.Build.props" redirects every project's own "bin/" and
# "obj/" into "build/<project folder name>/" instead (see Architecture)
build_output_directory: pathlib.Path = repository_root / 'build' / 'sample-app' / 'bin' / build_configuration / 'net10.0'
framework_assembly_path: pathlib.Path = build_output_directory / 'CLI.NET Core.dll'
framework_xml_documentation_path: pathlib.Path = build_output_directory / 'CLI.NET Core.xml'


def ensure_framework_is_built() -> None:
    """Builds the sample app (and, transitively, the framework) so that the framework's XML documentation file exists.

    Raises:
        RuntimeError: The XML documentation file is still missing after the build, which means GenerateDocumentationFile is not enabled on the
            framework project.
    """

    subprocess.run(
        ['dotnet', 'build', str(documented_project), '--configuration', build_configuration],
        cwd=repository_root,
        check=True,
    )

    if not framework_xml_documentation_path.is_file():
        raise RuntimeError(
            f'The XML documentation file was not found at "{framework_xml_documentation_path}" after building the project - '
            'GenerateDocumentationFile must be enabled on the framework\'s .csproj'
        )


def generate_markdown_into(output_directory: pathlib.Path) -> None:
    """Shells out to xmldoc2md, the chosen C# XML documentation to Markdown generator, writing its output into the given temporary directory.

    Args:
        output_directory: The temporary directory xmldoc2md should write its generated Markdown files into.
    """

    subprocess.run(
        [
            'dotnet',
            'tool',
            'run',
            'xmldoc2md',
            '--',
            str(framework_assembly_path),
            '--output',
            str(output_directory),
            '--structure',
            'tree',
            '--platform',
            'github-pages',
        ],
        cwd=repository_root,
        check=True,
    )


def page_title(markdown_content: str, fallback: str) -> str:
    """Extracts the H1 heading xmldoc2md put at the top of a generated page, so the navigation can show a type's real name (for example
    "CliCommandLineArguments") instead of the all-lowercase file name xmldoc2md derived it from.

    Args:
        markdown_content: The full text of one page xmldoc2md generated.
        fallback: The label to use if the page has no H1 heading, which should not happen for a page xmldoc2md generated.

    Returns:
        The heading text, or "fallback" if the page has no H1 heading.
    """

    heading_match = re.search(r'^# (.+)$', markdown_content, re.MULTILINE)
    return heading_match.group(1) if heading_match else fallback


def namespace_path(markdown_content: str) -> tuple[str, ...] | None:
    """Extracts the dotted namespace xmldoc2md put on every type's page (for example "Namespace: CliNetCore.Application") and splits it into the
    real, properly cased path segments the navigation should group that type under, instead of the all-lowercase folder names "--structure tree"
    derived them from. The root index page - the only page "--structure tree" writes without a "Namespace:" line - is handled by its caller
    instead, so it is not this function's concern.

    Args:
        markdown_content: The full text of one page xmldoc2md generated.

    Returns:
        The namespace's segments, most general first, or "None" if the page has no "Namespace:" line.
    """

    namespace_match = re.search(r'^Namespace: (.+)$', markdown_content, re.MULTILINE)
    return tuple(namespace_match.group(1).split('.')) if namespace_match else None


def inject_generated_pages(output_directory: pathlib.Path) -> None:
    """Copies every Markdown file xmldoc2md produced into the MkDocs build under "api/", using mkdocs_gen_files.open() so that none of it is ever
    written to the real "docs" directory on disk, and writes an "api/SUMMARY.md" alongside them that lists the same files as a literate nav (see
    docs/developer-manual/tooling/documentation-website.md) - this is what gets the "api" section into the site's navigation without a ".pages" file.

    Args:
        output_directory: The temporary directory xmldoc2md wrote its generated Markdown files into.
    """

    navigation = mkdocs_gen_files.Nav()

    for markdown_file in sorted(output_directory.rglob('*.md')):
        relative_path = markdown_file.relative_to(output_directory)
        markdown_content = markdown_file.read_text(encoding='utf-8')

        destination_path = pathlib.PurePosixPath('api') / relative_path
        with mkdocs_gen_files.open(destination_path, 'w', encoding='utf-8') as destination_file:
            destination_file.write(markdown_content)

        path_parts = relative_path.with_suffix('').parts
        if path_parts == ('index',):
            # The root index page: it has no "Namespace:" line for "namespace_path()" to read, and its H1 (the assembly's own name, "CLI.NET
            # Core") would be a confusing navigation label sitting alongside the type names - "Index" says what the page actually is.
            navigation[('Index',)] = relative_path.as_posix()
            continue

        navigation[(*(namespace_path(markdown_content) or path_parts[:-1]), page_title(markdown_content, fallback=path_parts[-1]))] = (
            relative_path.as_posix()
        )

    with mkdocs_gen_files.open(pathlib.PurePosixPath('api') / 'SUMMARY.md', 'w', encoding='utf-8') as navigation_file:
        navigation_file.writelines(navigation.build_literate_nav())


ensure_framework_is_built()
with tempfile.TemporaryDirectory(prefix='clinetcore-api-docs-') as temporary_directory_name:
    temporary_output_directory = pathlib.Path(temporary_directory_name)
    generate_markdown_into(temporary_output_directory)
    inject_generated_pages(temporary_output_directory)
