# Security Policy

## Supported Versions

CLI.NET Core is a young, actively developed project with a single maintained line of development. Only the most recently published release receives security fixes. There is no long-term-support version to backport to.

## Reporting a Vulnerability

**Do not open a public issue for a security vulnerability.** Instead, use GitHub's private vulnerability reporting for this repository: go to the **Security** tab and choose **Report a vulnerability**. If that is not available to you, contact the maintainer, [David Neumann](https://github.com/lecode-official), directly instead of filing a public issue.

Please include as much of the following as you can:

- The affected version (see the NuGet package's `<Version>`, or the commit hash).
- Steps to reproduce, or a minimal example that triggers the issue.
- The impact you believe the vulnerability has.

This is a project maintained by a single person in their spare time, so there is no formal Service Level Agreement (SLA) on response times, but reports are taken seriously and handled as promptly as possible. A fix, once available, is released and noted in [`CHANGELOG.md`](CHANGELOG.md). Credit is given to the reporter there unless anonymity is requested.

## Related

- How to contribute a fix once a vulnerability is disclosed: [`CONTRIBUTING.md`](CONTRIBUTING.md).
- Community standards for all interactions, including security reports: [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md).
