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
    internal class Binder
    {
        private readonly TypeSystem typeSystem;
        private readonly DiagnosticCollector diagnostics;
        private readonly Stack<SymbolScope> symbolStack;

        public Binder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
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

        private SyntaxNode GetNode(SyntaxToken token)
        {
            Debug.Assert(token is SyntaxNode);
            return (SyntaxNode)token;
        }

        private SyntaxTerminal GetTerminal(SyntaxToken token)
        {
            Debug.Assert(token is SyntaxTerminal);
            return (SyntaxTerminal)token;
        }

        private IEnumerable<SyntaxToken> Collect(SyntaxToken token, params SyntaxTokenKind[] kinds)
        {
            if (kinds.Contains(token.Kind))
            {
                yield return token;

                foreach (var child in token.Children())
                    foreach (var t in Collect(child, kinds))
                        yield return t;
            }
        }

        public BoundNode Bind(SyntaxToken token)
        {
            if (token is EmptySytaxToken)
                return new BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement>.Empty, ImmutableArray<BoundFunctionDefinition>.Empty);


            var functionSyntax = Collect(token, SyntaxTokenKind.CompilationUnit, SyntaxTokenKind.FunctionDefinition).Where(t => t.Kind == SyntaxTokenKind.FunctionDefinition);
            var globalSyntax = Collect(token, SyntaxTokenKind.CompilationUnit, SyntaxTokenKind.DeclarationStatement).Where(t => t.Kind == SyntaxTokenKind.DeclarationStatement);

            var functionTypes = new List<FunctionType>();
            var globals = ImmutableArray.CreateBuilder<BoundDeclarationStatement>();
            var functions = ImmutableArray.CreateBuilder<BoundFunctionDefinition>();

            foreach (var func in functionSyntax)
                functionTypes.Add(BindFunctionDeclaration(func));

            foreach (var global in globalSyntax)
                globals.Add(BindDeclarationStatement(global));

            foreach (var (funcType, funcDef) in functionTypes.Zip(functionSyntax))
                functions.Add(BindFunctionDefinition(funcDef, funcType));

            return new BoundCompilationUnit(globals.MoveToImmutable(), functions.MoveToImmutable());
        }


        private FunctionType BindFunctionDeclaration(SyntaxToken token)
        {
            // FunctionDefinition
            // ├── FuncKeyword
            // ├── Identifier
            // ├── LeftParen
            // ├── Parameters (optional)
            // ├── RightParen
            // ├── TypeClause (optional)
            // └── BlockStatement

            var functionDeclaration = (SyntaxNode)token;

            var nameTerminal = GetTerminal(functionDeclaration.Tokens[1]);
            var parameters = functionDeclaration.Tokens[3];
            var typeClause = functionDeclaration.Tokens[5];
            var body = GetNode(functionDeclaration.Tokens[6]);

            var name = nameTerminal.Text;

            var symbol = LookupSymbol(name);

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
                    diagnostics.NameAlreadyExistsError(nameTerminal.Location, name);
                functionGroup = new FunctionGroupSymbol(new FunctionGroupType(name));
            }

            var returnType = BindFunctionTypeClause(typeClause);

            var function = new FunctionType(name, functionGroup.Group, returnType);

            var paramSyntax = Collect(parameters, SyntaxTokenKind.Parameters, SyntaxTokenKind.ParameterDeclaration).Where(t => t.Kind == SyntaxTokenKind.ParameterDeclaration).ToList(); 
            
            throw new NotImplementedException();
        }

        private TypeSpecifier BindFunctionTypeClause(SyntaxToken typeClause)
        {
            if (typeClause is EmptySytaxToken)
                return typeSystem.Void;

            var node = GetNode(typeClause);

            var nameTerminal = GetTerminal(node.Tokens[1]);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return typeSystem.Error;
            }
            else
            {
                return symbol.Type;
            }
        }

        private BoundFunctionDefinition BindFunctionDefinition(SyntaxToken funcDef, FunctionType funcType)
        {
            throw new NotImplementedException();
        }

        private BoundDeclarationStatement BindDeclarationStatement(SyntaxToken global)
        {
            throw new NotImplementedException();
        }

    }
}