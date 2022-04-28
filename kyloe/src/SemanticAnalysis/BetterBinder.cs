using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Kyloe.Diagnostics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

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

        private SyntaxNode GetNode(SyntaxToken token, SyntaxTokenKind kind)
        {
            Debug.Assert(token is SyntaxNode);
            Debug.Assert(token.Kind == kind);
            return (SyntaxNode)token;
        }

        private SyntaxTerminal GetTerminal(SyntaxToken token)
        {
            Debug.Assert(token is SyntaxTerminal);
            return (SyntaxTerminal)token;
        }

        private SyntaxTerminal GetTerminal(SyntaxToken token, SyntaxTokenKind kind)
        {
            Debug.Assert(token is SyntaxTerminal);
            Debug.Assert(token.Kind == kind);
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
            if (token.Kind == SyntaxTokenKind.Epsilon)
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

            var functionDeclaration = GetNode(token, SyntaxTokenKind.FunctionDefinition);

            var nameTerminal = GetTerminal(functionDeclaration.Tokens[1], SyntaxTokenKind.Identifier);
            var parameters = GetNode(functionDeclaration.Tokens[3]);
            var typeClause = GetNode(functionDeclaration.Tokens[5]);
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

            var paramSyntax = Collect(parameters, SyntaxTokenKind.Parameters, SyntaxTokenKind.ParameterDeclaration).Where(t => t.Kind == SyntaxTokenKind.ParameterDeclaration);

            foreach (var param in paramSyntax)
                function.Parameters.Add(BindParameter(param));

            bool alreadyExists = false;

            foreach (var otherFunction in functionGroup.Group.Functions)
            {
                if (TypeSequenceEquals(function.Parameters.Select(param => param.Type), otherFunction.Parameters.Select(param => param.Type)))
                {
                    if (symbol is not ErrorSymbol)
                        diagnostics.OverloadWithSameParametersExistsError(nameTerminal.Location, name);
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                functionGroup.Group.Functions.Add(function);

            return function;
        }

        private ParameterSymbol BindParameter(SyntaxToken token)
        {
            // ParameterDeclaration
            // ├── Identifier
            // └── TypeClause

            var parameter = GetNode(token, SyntaxTokenKind.ParameterDeclaration);

            var nameTerminal = GetTerminal(parameter.Tokens[0], SyntaxTokenKind.Identifier);
            var typeClause = GetNode(parameter.Tokens[0]);

            var type = BindTypeClause(typeClause);

            return new ParameterSymbol(nameTerminal.Text, type, nameTerminal.Location);
        }

        private TypeSpecifier BindFunctionTypeClause(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return typeSystem.Void;

            if (token.Kind == SyntaxTokenKind.Error)
                return typeSystem.Error;

            var typeClause = GetNode(token, SyntaxTokenKind.TrailingTypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return typeSystem.Error;
            }

            return symbol.Type;
        }

        private TypeSpecifier BindTypeClause(SyntaxToken token)
        {
            // TypeClause
            // ├── Colon
            // └── Identifier

            if (token.Kind == SyntaxTokenKind.Error)
                return typeSystem.Error;

            var typeClause = GetNode(token, SyntaxTokenKind.TypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return typeSystem.Error;
            }

            return symbol.Type;
        }

        private BoundFunctionDefinition BindFunctionDefinition(SyntaxToken token, FunctionType type)
        {
            // FunctionDefinition
            // ├── FuncKeyword
            // ├── Identifier
            // ├── LeftParen
            // ├── Parameters (optional)
            // ├── RightParen
            // ├── TypeClause (optional)
            // └── BlockStatement

            var function = GetNode(token, SyntaxTokenKind.FunctionDefinition);

            EnterNewScope(); // this scope contains the parameters

            foreach (var param in type.Parameters)
                if (!DeclareSymbol(param))
                    diagnostics.RedefinedParameterError((SourceLocation)param.Loaction!, param.Name);

            var body = GetNode(function.Tokens[6]);
            var boundBody = BindBlockStatement(body);

            ExitCurrentScope();

            return new BoundFunctionDefinition(type, boundBody);
        }

        private BoundStatement BindStatement(SyntaxToken token)
        {
            switch (token.Kind)
            {
                case SyntaxTokenKind.Error:
                    return new BoundInvalidStatement();
                case SyntaxTokenKind.SemiColon:
                    return new BoundEmptyStatement();
                case SyntaxTokenKind.DeclarationStatement:
                    return BindDeclarationStatement(token);
                case SyntaxTokenKind.BlockStatement:
                    return BindBlockStatement(token);
                case SyntaxTokenKind.ExpressionStatement:
                    return BindExpressionStatement(token);
                case SyntaxTokenKind.IfStatement:
                    return BindIfStatement(token);
                default:
                    throw new Exception($"unexpected kind: {token.Kind}");
            }
        }

        private BoundStatement BindIfStatement(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundStatement BindExpressionStatement(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundDeclarationStatement BindDeclarationStatement(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundBlockStatement BindBlockStatement(SyntaxToken token)
        {
            // BlockStatement
            // ├── LeftCurly
            // ├── RepeatedStatement
            // └── RightCurly

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundBlockStatement(ImmutableArray<BoundStatement>.Empty);

            var block = GetNode(token, SyntaxTokenKind.BlockStatement);

            var body = block.Tokens[1];

            var statements = CollectStatements(body);

            var builder = ImmutableArray.CreateBuilder<BoundStatement>();


            foreach (var stmt in statements)
                builder.Add(BindStatement(stmt));

            return new BoundBlockStatement(builder.ToImmutable());
        }

        private IEnumerable<SyntaxToken> CollectStatements(SyntaxToken token)
        {
            // RepeatedStatement
            // ├── RepeatedStatement (optional)
            // └── Statement

            if (token.Kind == SyntaxTokenKind.Error || token.Kind == SyntaxTokenKind.Epsilon)
                yield break;

            var repeat = GetNode(token, SyntaxTokenKind.RepeatedStatement);

            foreach (var stmt in CollectStatements(repeat.Tokens[0]))
                yield return stmt;

            yield return repeat.Tokens[1];
        }


        private BoundExpression BindExpression(SyntaxToken token)
        {
            switch (token.Kind)
            {
                case SyntaxTokenKind.Error:
                    return new BoundInvalidExpression(typeSystem);
                case SyntaxTokenKind.AssignmentHelper:
                    return BindAssignmentHelper(token);
                case SyntaxTokenKind.LogicalOr:
                case SyntaxTokenKind.LogicalAnd:
                case SyntaxTokenKind.BitOr:
                case SyntaxTokenKind.BitXor:
                case SyntaxTokenKind.BitAnd:
                case SyntaxTokenKind.Equality:
                case SyntaxTokenKind.Comparison:
                case SyntaxTokenKind.Sum:
                case SyntaxTokenKind.Mult:
                    return BindBinary(token);
                case SyntaxTokenKind.Prefix:
                    return BindPrefix(token);
                case SyntaxTokenKind.Postfix:
                    return BindPostfix(token);
                case SyntaxTokenKind.Parenthesized:
                    return BindParenthesized(token);
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                    return BindLiteral(token);

                default:
                    throw new Exception($"unexpected kind: {token.Kind}");
            }
        }


        private BoundExpression BindAssignmentHelper(SyntaxToken token)
        {
            // AssignmentHelper
            // ├── Expr
            // └── Assignment (optional)

            var helper = GetNode(token, SyntaxTokenKind.AssignmentHelper);

            var rhs = BindExpression(helper.Tokens[0]);

            if (helper.Tokens[1].Kind == SyntaxTokenKind.Epsilon)
                return rhs;

            throw new NotImplementedException();
        }

        private BoundExpression BindBinary(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindPrefix(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindPostfix(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindParenthesized(SyntaxToken token)
        {
            // Parenthesized
            // ├── LeftParen
            // ├── Expression
            // └── RightParen

            var node = GetNode(token, SyntaxTokenKind.Parenthesized);
            return BindExpression(node.Tokens[1]);
        }

        private BoundExpression BindLiteral(SyntaxToken token)
        {
            throw new NotImplementedException();
        }
    }
}