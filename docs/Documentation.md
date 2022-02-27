# Kyloe Documentation

This is the documentation for the kyloe core library.

The library consists these main parts:
- [SyntaxAnalysis](#syntax-analysis)
- [SemanticAnalysis](#semantic-analysis)
- [Diagnostics](#diagnostics)

As well as utility types [Utility](#utility) and the main public interfaces [Compilation](../kyloe/src/Compilation.cs) and [SyntaxTree](../kyloe/src/SyntaxTree.cs)


## Syntax Analysis

The syntax analysis is done by the two main classes [Lexer](../kyloe/src/SyntaxAnalysis/Lexer.cs) [Parser](../kyloe/src/SyntaxAnalysis/Parser.cs).

### Lexer
The job of the lexer is to transform the text read from a `System.IO.TextReader` into a stream of [SyntaxTokens](../kyloe/src/SyntaxAnalysis/SyntaxToken.cs).

## Semantic Analysis
TODO
## Diagnostics
TODO
## Utility
TODO
