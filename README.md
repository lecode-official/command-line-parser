# Command Line Parser

![Command Line Parser Logo](docs/images/Banner.png "Command Line Parser Logo")

A simple and light-weight parser for command line arguments, which makes it possible to access parameters
from the command line via well-defined .NET data structures.

## Using the Project

You can always get the latest stable version from NuGet:

```bash
Install-Package System.CommandLine.Parser -Version 0.1.1     # Using the Visual Studio Package Manager
dotnet add package System.CommandLine.Parser --version 0.1.1 # Using the .NET Command Line Interface
paket add System.CommandLine.Parser --version 0.1.1          # Using the packet Command Line Interface
```

**__But please be aware that the current 0.1.1 version on NuGet is outdated and should only be used for
legacy projects. The next release is a complete rewrite of the original code for .NET Core and will work
on all supported .NET Core platforms (e.g. Windows, macOS, and Linux).__**

If you want to go ahead and use the new version, you can download and manually build the solution. The
project was built using Visual Studio Code and .NET Core 2.0. To build the solution, clone the repository
and build it using the command line tools of .NET Core:

```bash
git clone https://github.com/lecode-official/command-line-parser
cd command-line-parser
dotnet build
dotnet test ./test/System.CommandLine.Parser.Tests/System.CommandLine.Parser.Tests.csproj
```

## Contributions

I always greatly appreciate feedback and bug reports. To file a bug, please use GitHub's issue system.
Alternatively, you can clone the repository and send me a pull request.
