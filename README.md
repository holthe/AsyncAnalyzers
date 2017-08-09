# AsyncAnalyzers
> This repository contains a TAP method naming diagnostic extension for the .NET Compiler Platform ("Roslyn").

[![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=master)](https://travis-ci.org/holthe/AsyncAnalyzers)
[![Latest version](https://img.shields.io/nuget/v/AsyncAnalyzers.svg)](https://www.nuget.org/packages/AsyncAnalyzers)
[![License](http://img.shields.io/:license-MIT-red.svg)](LICENSE.md)

## Build Status

Compilation and build status is provided by [Travis CI](https://travis-ci.org). The **VSIX** project is not being built at the moment.

Branch|Status
---|---
master|[![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=master)](https://travis-ci.org/holthe/AsyncAnalyzers)
develop|[![Build Status](https://travis-ci.org/holthe/AsyncAnalyzers.svg?branch=develop)](https://travis-ci.org/holthe/AsyncAnalyzers)

## Getting Started

1. Clone the repository and build the solution (optionally with `rake`).
2. Add the analyzer to a given solution by copying the `AsyncAnalyzers.dll` and add a reference to it under References/Analyzers for each of the projects in the solution.

### Prerequisites

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

which assumes that the solution has previously been built for **Release** configuration and that artifacts from this build is present in the output folders for the _AsyncAnalyzers_ and _AsyncAnalyzers.Test_ projects.

## License

This project is licensed under the MIT License - see [![License](http://img.shields.io/:license-MIT-red.svg)](LICENSE.md) for details.
