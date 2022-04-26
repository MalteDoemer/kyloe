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

        public SyntaxNode GetNode(SyntaxToken? token) => (SyntaxNode)token!;

        public SyntaxTerminal GetTerminal(SyntaxToken? token) => (SyntaxTerminal)token!;


        public BoundNode Bind(SyntaxToken? token)
        {
            Debug.Assert(token is not null);
            Debug.Assert(token.Kind == SyntaxTokenKind.CompilationUnit);
            return BindCompilationUnit((SyntaxNode)token!);
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

            foreach (var global in globalSyntax)
                globals.Add(BindDeclarationStatement(global));

            foreach (var (type, func) in functionTypes.Zip(functionSyntax))
                functions.Add(BindFunctionDefinition(type, func));

            return new BoundCompilationUnit(globals.ToImmutable(), functions.ToImmutable());
        }

        private IEnumerable<SyntaxNode> GetFunctionDefinitions(SyntaxNode compilationUnit)
        {
            // CompilationUnit
            //      - CompilationUnit
            //      - Declaration/Function

            var unit = compilationUnit.Tokens[0];

            if (IsTokenKind(SyntaxTokenKind.CompilationUnit, unit))
                foreach (var token in GetFunctionDefinitions(GetNode(unit)))
                    yield return token;

            var child = compilationUnit.Tokens[1];
            if (IsTokenKind(SyntaxTokenKind.FunctionDefinition, child))
                yield return GetNode(child);

        }

        private IEnumerable<SyntaxNode> GetGlobalDeclarations(SyntaxNode compilationUnit)
        {
            // CompilationUnit
            //      - CompilationUnit
            //      - Declaration/Function

            var unit = compilationUnit.Tokens[0];

            if (IsTokenKind(SyntaxTokenKind.CompilationUnit, unit))
                foreach (var token in GetGlobalDeclarations(GetNode(unit)))
                    yield return token;

            var child = compilationUnit.Tokens[1];
            if (IsTokenKind(SyntaxTokenKind.DeclarationStatement, child))
                yield return GetNode(child);

        }

        private FunctionType BindFunctionDeclaration(SyntaxNode func)
        {
            // FunctionDeclaration
            //      - func
            //      - identifier
            //      - (
            //      - parameters (optionally)
            //      - )
            //      - type clause (optionally)
            //      - block


            var nameTerminal = GetTerminal(SyntaxTokenKind.Identifier, func.Tokens[1]);
            var typeClause = func.Tokens[5];
            var parameters = func.Tokens[3];

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
                returnType = BindTypeClause(clause);
            }

            var function = new FunctionType(name, functionGroup.Group, returnType);

            foreach (var param in GetParameters(parameters as SyntaxNode))
                function.Parameters.Add(BindParameter(param));

            bool alreadyExists = false;

            foreach (var otherFunction in functionGroup.Group.Functions)
            {
                if (TypeSequenceEquals(function.Parameters.Select(param => param.Type), otherFunction.Parameters.Select(param => param.Type)))
                {
                    if (symbol is not ErrorSymbol)
                        diagnostics.Add(new OverloadWithSameParametersExistsError(nameTerminal));
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                functionGroup.Group.Functions.Add(function);

            return function;
        }

        private BoundFunctionDefinition BindFunctionDefinition(FunctionType funcType, SyntaxNode funcDef)
        {
            // FunctionDefinition
            //      - func
            //      - identifier
            //      - (
            //      - parameters (optionally)
            //      - )
            //      - type clause (optionally)
            //      - block

            EnterNewScope(); // this scope contains the parameters

            var name = GetTerminal(SyntaxTokenKind.Identifier, funcDef.Tokens[1]);
            var body = GetNode(SyntaxTokenKind.BlockStatement, funcDef.Tokens[6]);

            foreach (var param in funcType.Parameters)
                if (!DeclareSymbol(param))
                    diagnostics.Add(new RedefinedParameterError(name)); // FIXME: this is wrong, we have to get the location of the parameter name

            var boundBody = BindBlockStatement(body);

            ExitCurrentScope();

            return new BoundFunctionDefinition(funcType, boundBody);
        }

        private IEnumerable<SyntaxNode> GetParameters(SyntaxNode? parameters)
        {

            // (null)
            //
            // ParameterDeclaration
            //            
            // Parameters
            //      - Comma
            //      - ParameterDeclaration
            //
            // Parameters
            //      - Parameters/ParameterDeclaration
            //      - Parameters/ParameterDeclaration

            if (parameters is null)
                yield break;

            if (parameters.Kind == SyntaxTokenKind.ParameterDeclaration)
                yield return parameters;

            var left = parameters.Tokens[0];
            var right = GetNode(parameters.Tokens[0]);


            if (IsTokenKind(SyntaxTokenKind.Comma, left))
            {
                foreach (var child in GetParameters(GetNode(right)))
                    yield return child;
            }
            else
            {
                foreach (var child in GetParameters(GetNode(left)))
                    yield return child;

                foreach (var child in GetParameters(GetNode(right)))
                    yield return child;
            }
        }

        private ParameterSymbol BindParameter(SyntaxNode param)
        {
            // ParameterDeclaration
            //      - Identifier
            //      - TypeClause

            var nameTerminal = GetTerminal(SyntaxTokenKind.Identifier, param.Tokens[0]);
            var typeClause = GetNode(SyntaxTokenKind.TypeClause, param.Tokens[1]);

            var bound = BindTypeClause(typeClause);

            return new ParameterSymbol(nameTerminal.Text, bound);
        }

        private TypeSpecifier BindTypeClause(SyntaxNode node)
        {
            // TypeClause
            //      - :
            //      - identifier

            // TrailingTypeClause
            //      - ->
            //      - identifier


            var nameTerminal = GetTerminal(SyntaxTokenKind.Identifier, node.Tokens[1]);
            var typeSymbol = LookupSymbol(nameTerminal.Text);

            if (typeSymbol is null)
            {
                diagnostics.Add(new NonExistantNameError(nameTerminal));
                return typeSystem.Error;
            }

            return typeSymbol.Type;
        }

        private BoundStatement BindStatement(SyntaxToken token)
        {
            switch (token.Kind)
            {
                case SyntaxTokenKind.SemiColon:
                    return new BoundEmptyStatement();
                case SyntaxTokenKind.BlockStatement:
                    return BindBlockStatement(GetNode(SyntaxTokenKind.BlockStatement, token));
                case SyntaxTokenKind.ExpressionStatement:
                    return BindExpressionStatement(GetNode(SyntaxTokenKind.ExpressionStatement, token));
                case SyntaxTokenKind.IfStatement:
                    return BindIfStatement(GetNode(SyntaxTokenKind.IfStatement, token));
                case SyntaxTokenKind.DeclarationStatement:
                    return BindDeclarationStatement(GetNode(SyntaxTokenKind.DeclarationStatement, token));

                default: throw new Exception($"unexpected token kind: {token.Kind}");
            }
        }

        private BoundDeclarationStatement BindDeclarationStatement(SyntaxNode syntaxNode)
        {
            // DeclarationStatement
            //      - var/const
            //      - identifier
            //      - = 
            //      - expr
            //      - ;

            throw new NotImplementedException();
        }

        private BoundIfStatement BindIfStatement(SyntaxNode syntaxNode)
        {
            throw new NotImplementedException();
        }

        private BoundExpressionStatement BindExpressionStatement(SyntaxNode syntaxNode)
        {
            throw new NotImplementedException();
        }

        private BoundBlockStatement BindBlockStatement(SyntaxNode blockStatement)
        {
            // BlockStatement 
            //      - {
            //      - RepeatedStatement (optionally)
            //      - }

            var repeatedStatement = blockStatement.Tokens[1] as SyntaxNode;

            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (var stmt in GetStatements(repeatedStatement))
                statements.Add(BindStatement(stmt));

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private IEnumerable<SyntaxToken> GetStatements(SyntaxNode? repeatedStatement)
        {
            if (repeatedStatement is null)
                yield break;

            // RepeatedStatement
            //      - RepeatedStatement (optionally)
            //      - Statement

            var left = repeatedStatement.Tokens[0];
            var right = repeatedStatement.Tokens[1];

            foreach (var stmt in GetStatements(left as SyntaxNode))
                yield return stmt;

            yield return GetNode(right);
        }
    }
}