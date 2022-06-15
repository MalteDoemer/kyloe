# Kyloe
A compiler written in C# for my custom programming language named kyloe.


## Projects
- [kyc](kyc) The command line interface for kyloe.
- [kyloe](kyloe) The core library that handles the compilation.
- [grammar](grammar) A grammar analyzer and parser generator that is used for the core library.
- [codegen](codegen) A library to generate C# code (not complete) that is used by the parser generator.

## Running the Examples
Build the whole project:

```dotnet build```

Change directory to the folder containing the example:

```cd examples/test```

Run the example:

```dotnet run```

## Documentation
Documentation can be found in [Documentation.md](docs/Documentation.md)