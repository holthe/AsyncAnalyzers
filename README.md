# AsyncAnalyzers [![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=master)](https://travis-ci.org/holthe/AsyncAnalyzers) [![Latest version](https://img.shields.io/nuget/v/AsyncAnalyzers.svg)](https://www.nuget.org/packages/AsyncAnalyzers) [![NuGet](https://img.shields.io/nuget/dt/AsyncAnalyzers.svg)](https://www.nuget.org/packages/AsyncAnalyzers) [![License](http://img.shields.io/:license-MIT-red.svg)](LICENSE.md)
> This repository contains a set of diagnostic extensions for the .NET Compiler Platform ("Roslyn") regarding asynchronous practices.

## Build Status

Compilation and build status are provided by [Travis CI](https://travis-ci.org). The **VSIX** project is not being built at the moment.

Branch|Status
---|---
master|[![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=master)](https://travis-ci.org/holthe/AsyncAnalyzers)
develop|[![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=develop)](https://travis-ci.org/holthe/AsyncAnalyzers)

## Getting Started

### Install NuGet package (preferred method)

To install **AsyncAnalyzers**, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```
Install-Package AsyncAnalyzers
```

or the following command for [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)
```
dotnet add package AsyncAnalyzers
```

### Install from GitHub Releases

[Travis CI](https://travis-ci.org) deploys the latest NuGet package to [GitHub Releases](https://github.com/holthe/AsyncAnalyzers/releases) along with its SHA-256 hash after it has been pushed to the [NuGet Gallery](https://www.nuget.org/packages/AsyncAnalyzers).

1. Download the [latest release](https://github.com/holthe/AsyncAnalyzers/releases) (`.nupkg` and `.sha256` files) and check the SHA-256 hash of the downloaded `.nupkg` file, e.g. for release v1.1.5:
```
sha256sum -c AsyncAnalyzers.1.1.5.0.nupkg.sha256
```

which should output `AsyncAnalyzers.1.1.5.0.nupkg: OK`.

2. Install the downloaded package
```
Install-Package C:\[PathToThePackageDir]\AsyncAnalyzers.1.1.5.0.nupkg
```

### Install from source

1. Clone the repository and build the solution (optionally with `rake`).
2. Add the analyzer to a given solution by copying the `AsyncAnalyzers.dll` and add a reference to it under _References/Analyzers_ for each of the projects in the solution.

#### Prerequisites

In order to build this solution using `rake`, you need to install `albacore` version 2.6.1:
```
gem install albacore -v 2.6.1
```

Note that this requires ruby version >= 2.1.0.

## Running the tests

The tests are written using the `xUnit` testing framework and can be run directly from within **Visual Studio** or from the commandline using `rake`. The `default` task in `Rakefile` run the tests after performing a **NuGet** package restore and building the solution for **Release**, so one can simply issue the `rake` command. Alternately, run the `xunit_tests` task directly:
```
rake xunit_tests
```

which assumes that the solution has previously been built for **Release** configuration and that artifacts from this build are present in the output folders for the _AsyncAnalyzers_ and _AsyncAnalyzers.Test_ projects.

## License [![License](http://img.shields.io/:license-MIT-red.svg)](LICENSE.md)

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
