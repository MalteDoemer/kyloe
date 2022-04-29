# Kyloe Documentation

This is the documentation for the kyloe core library.

The library consists these main parts:
- [SyntaxAnalysis](#syntax-analysis)
- [SemanticAnalysis](#semantic-analysis)
- [Diagnostics](#diagnostics)

As well as utility types [Utility](#utility) and the main public interfaces [Compilation](../kyloe/src/Compilation.cs) and [SyntaxTree](../kyloe/src/SyntaxTree.cs)


## Syntax Analysis

The syntax analysis is done by the two main classes [Lexer](../kyloe/src/SyntaxAnalysis/Generated/Lexer.cs) [Parser](../kyloe/src/SyntaxAnalysis/Generated/Parser.cs).

### Lexer
The job of the lexer is to transform the text  into a stream of [SyntaxTerminals](../kyloe/src/SyntaxAnalysis/Generated/SyntaxTerminal.cs).

### Parser
The parser reads the terminals from the lexer and builds the parse tree, consiting of [SyntaxTerminals](../kyloe/src/SyntaxAnalysis/Generated/SyntaxTerminal.cs) and [SyntaxNodes](../kyloe/src/SyntaxAnalysis/Generated/SyntaxNode.cs). The parser is a recursive descent parser, with the additional ability to parse left recursive productions using while loops.

## Semantic Analysis
TODO
## Diagnostics
TODO
## Utility
TODO
