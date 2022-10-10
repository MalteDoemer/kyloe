# Kyloe
A compiler written in C# for my custom programming language named kyloe.


## Projects
- [kyc](kyc) The command line interface for kyloe.
- [kyloe](kyloe) The core library that handles the compilation.
- [kyloe.builtins](kyloe.builtins) A library that contains all built-in functions that can be used from a kyloe program.
- [kyloe.tests](kyloe.tests) Tests for the core library.
- [kyloe.bench](kyloe.tests) Benchmarks for the core library.
- [grammar](grammar) A grammar analyzer and parser generator that is used for the core library.
- [codegen](codegen) A library to generate C# code (not complete) that is used by the parser generator.

## Prerequisites

The .NET SDK has to be installed on your system. For install instructions see https://dotnet.microsoft.com/en-us/download.

## Running the Examples

First download the Project using git or as a [zip file](https://github.com/MalteDoemer/kyloe/zipball/main).

Then open the Project in a Terminal and build it:

```dotnet build```

Change directory to the folder containing the example:

```cd examples/hello-world```

Build the example with:

```dotnet build```

And execute it with:

```dotnet run```

## Documentation

Documentation can be found in [Documentation.md](docs/Documentation.md)

## License

Licensed under the MIT license ([LICENSE](LICENSE) or http://opensource.org/licenses/MIT)