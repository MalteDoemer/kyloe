using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Kyloe.Diagnostics;
using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal class BetterBinder
    {
        private readonly TypeSystem typeSystem;
        private readonly DiagnosticCollector diagnostics;
        private readonly Stack<SymbolScope> symbolStack;

        public BetterBinder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
        {
            this.typeSystem = typeSystem;
            this.diagnostics = diagnostics;
            this.symbolStack = new Stack<SymbolScope>();
            this.symbolStack.Push(typeSystem.GlobalScope);
        }

        private void EnterNewScope()
        {
            symbolStack.Push(new SymbolScope());
        }

        private void ExitCurrentScope()
        {
            symbolStack.Pop();
        }

        private bool DeclareSymbol(Symbol symbol)
        {
            return symbolStack.Peek().DeclareSymbol(symbol);
        }

        private Symbol? LookupSymbol(string name)
        {
            foreach (var scope in symbolStack)
                if (scope.LookupSymbol(name) is Symbol symbol)
                    return symbol;

            return null;
        }

        private bool InGlobalScope() => symbolStack.Count == 1;

        private bool IsTypeMissmatch(TypeSpecifier expectedType, TypeSpecifier rightType)
        {
            if (expectedType is ErrorType || rightType is ErrorType)
                return false;

            return !expectedType.Equals(rightType);
        }

        private bool TypeSequenceEquals(IEnumerable<TypeSpecifier> seq1, IEnumerable<TypeSpecifier> seq2)
        {
            if (seq1.Count() != seq2.Count())
                return false;

            foreach (var (t1, t2) in seq1.Zip(seq2))
                if (!t1.Equals(t2))
                    return false;

            return true;
        }

        private bool IsTokenKind(SyntaxTokenKind kind, SyntaxToken? token) => token is not null && token.Kind == kind;

        public SyntaxNode GetNode(SyntaxTokenKind kind, SyntaxToken? token)
        {
            if (token is not null && token.Kind == SyntaxTokenKind.Error)
                return (SyntaxNode)token!;

            Debug.Assert(IsTokenKind(kind, token));
            Debug.Assert(kind.IsNonTerminal());
            return (SyntaxNode)token!;
        }

        public SyntaxTerminal GetTerminal(SyntaxTokenKind kind, SyntaxToken? token)
        {
            Debug.Assert(IsTokenKind(kind, token));
            Debug.Assert(kind.IsTerminal());
            return (SyntaxTerminal)token!;
        }


        public BoundNode Bind(SyntaxToken token)
        {
            Debug.Assert(token.Kind == SyntaxTokenKind.CompilationUnit);
            return BindCompilationUnit((SyntaxNode)token);
        }

        private BoundNode BindCompilationUnit(SyntaxNode compilationUnit)
        {
            var functionSyntax = GetFunctionDefinitions(compilationUnit).ToList();
            var globalSyntax = GetGlobalDeclarations(compilationUnit).ToList();

            var functionTypes = new List<FunctionType>(functionSyntax.Count);
            var functions = ImmutableArray.CreateBuilder<BoundFunctionDefinition>(functionSyntax.Count);
            var globals = ImmutableArray.CreateBuilder<BoundDeclarationStatement>(globalSyntax.Count);


            // forward declare all functions
            foreach (var func in functionSyntax)
                functionTypes.Add(BindFunctionDeclaration(func));


            throw new NotImplementedException();
        }

        private IEnumerable<SyntaxNode> GetFunctionDefinitions(SyntaxNode compilationUnit)
        {
            // CompilationUnit
            //      - Declaration/Function

            // CompilationUnit
            //      - CompilationUnit
            //      - Declaration/Function


            if (compilationUnit.Tokens.Length == 1)
            {
                var child = compilationUnit.Tokens[0];
                if (IsTokenKind(SyntaxTokenKind.FunctionDefinition, child))
                    yield return (SyntaxNode)child!;
            }
            else
            {
                var unit = compilationUnit.Tokens[0];

                if (IsTokenKind(SyntaxTokenKind.CompilationUnit, unit))
                    foreach (var token in GetFunctionDefinitions((SyntaxNode)unit!))
                        yield return token;

                var child = compilationUnit.Tokens[1];
                if (IsTokenKind(SyntaxTokenKind.FunctionDefinition, child))
                    yield return (SyntaxNode)child!;
            }
        }

        private IEnumerable<SyntaxNode> GetGlobalDeclarations(SyntaxNode compilationUnit)
        {
            // CompilationUnit
            //      - Declaration/Function

            // CompilationUnit
            //      - CompilationUnit
            //      - Declaration/Function


            if (compilationUnit.Tokens.Length == 1)
            {
                var child = compilationUnit.Tokens[0];
                if (IsTokenKind(SyntaxTokenKind.DeclarationStatement, child))
                    yield return (SyntaxNode)child!;
            }
            else
            {
                var unit = compilationUnit.Tokens[0];

                if (IsTokenKind(SyntaxTokenKind.CompilationUnit, unit))
                    foreach (var token in GetGlobalDeclarations((SyntaxNode)unit!))
                        yield return token;

                var child = compilationUnit.Tokens[1];
                if (IsTokenKind(SyntaxTokenKind.DeclarationStatement, child))
                    yield return (SyntaxNode)child!;
            }
        }

        private FunctionType BindFunctionDeclaration(SyntaxNode func)
        {
            // FunctionDeclaration
            //      - func
            //      - identifier
            //      - (
            //      - parameters
            //      - )
            //      - type clause (optionally)
            //      - block


            var nameTerminal = GetTerminal(SyntaxTokenKind.Identifier, func.Tokens[1]);
            var typeClause = func.Tokens[5];

            var name = nameTerminal.Text;

            var symbol = LookupSymbol(nameTerminal.Text);

            FunctionGroupSymbol functionGroup;

            if (symbol is null)
            {
                functionGroup = new FunctionGroupSymbol(new FunctionGroupType(name));
                DeclareSymbol(functionGroup);
            }
            else if (symbol is FunctionGroupSymbol groupSymbol)
            {
                functionGroup = groupSymbol;
            }
            else
            {
                if (symbol is not ErrorSymbol)
                    diagnostics.Add(new NameAlreadyExistsError(nameTerminal));
                functionGroup = new FunctionGroupSymbol(new FunctionGroupType(name));
            }

            TypeSpecifier returnType;

            if (typeClause is null)
                returnType = typeSystem.Void;
            else
            {
                var clause = GetNode(SyntaxTokenKind.TrailingTypeClause, typeClause);
                returnType = BindTrailingTypeClause(clause).Type;
            }

            var function = new FunctionType(name, functionGroup.Group, returnType);


        }

        private IEnumerable<SyntaxNode> GetParameters(SyntaxNode parameters) 
        {
            // Parameters
            //      - ParameterDeclaration

            // Parameters
            //      - Comma
            //      - ParameterDeclaration 
        }

        private BoundTypeClause BindTrailingTypeClause(SyntaxNode node)
        {
            // TrailingTypeClause
            //      - ->
            //      - identifier

            var nameTerminal = GetTerminal(SyntaxTokenKind.Identifier, node.Tokens[1]);

            throw new NotImplementedException();
        }
    }
}