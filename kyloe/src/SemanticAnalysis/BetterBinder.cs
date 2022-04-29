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

        private MethodType? FindMethodOverload(MethodGroupType group, IEnumerable<TypeSpecifier> parameterTypes, bool isStatic)
        {
            foreach (var method in group.Methods)
            {
                if (method.IsStatic == isStatic && TypeSequenceEquals(method.Parameters.Select(param => param.Type), parameterTypes))
                    return method;
            }

            return null;
        }

        private TypeSpecifier GetResultType(BoundExpression expr, SourceLocation src, bool mustBeValue, bool mustBeModifiableValue, bool mustBeTypeName)
        {
            if (expr.ResultType is ErrorType)
                return typeSystem.Error;

            if (mustBeTypeName && !expr.IsTypeName)
            {
                diagnostics.ExpectedTypeNameError(src);
                return typeSystem.Error;
            }

            if (mustBeValue && !expr.IsValue)
            {
                diagnostics.ExpectedValueError(src);
                return typeSystem.Error;
            }

            if (mustBeModifiableValue && !expr.IsModifiableValue)
            {
                diagnostics.ExpectedModifiableValueError(src);
                return typeSystem.Error;
            }

            return expr.ResultType;
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

            return new BoundCompilationUnit(globals.ToImmutable(), functions.ToImmutable());
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
                if (symbol is not ErrorSymbol && !nameTerminal.Invalid)
                    diagnostics.NameAlreadyExistsError(nameTerminal.Location, name);
                functionGroup = new FunctionGroupSymbol(new FunctionGroupType(name));
            }

            var returnType = BindFunctionTypeClause(typeClause);

            var function = new FunctionType(name, functionGroup.Group, returnType);

            var paramSyntax = Collect(parameters, SyntaxTokenKind.Parameters, SyntaxTokenKind.ParameterDeclaration).Where(t => t.Kind == SyntaxTokenKind.ParameterDeclaration);

            foreach (var param in paramSyntax)
            {
                // ParameterDeclaration
                // ├── Identifier
                // └── TypeClause

                var parameter = GetNode(param, SyntaxTokenKind.ParameterDeclaration);
                var paramNameTerminal = GetTerminal(parameter.Tokens[0], SyntaxTokenKind.Identifier);
                var type = BindTypeClause(GetNode(parameter.Tokens[1]));
                var paramSymbol = new ParameterSymbol(nameTerminal.Text, type);

                if (!paramNameTerminal.Invalid)
                    function.Parameters.Add(paramSymbol);
            }

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
                if (!nameTerminal.Invalid)
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
                if (!nameTerminal.Invalid)
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
                    if (param.Location is SourceLocation loc)
                        diagnostics.RedefinedParameterError(loc, param.Name);

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
            // ExpressionStatement
            // ├── Expression
            // └── SemiColon

            var statement = GetNode(token, SyntaxTokenKind.ExpressionStatement);
            var bound = BindExpression(statement.Tokens[0]);
            return new BoundExpressionStatement(bound);
        }

        private BoundDeclarationStatement BindDeclarationStatement(SyntaxToken token)
        {
            // DeclarationStatement
            // ├── VarKeyword/ConstKeyword
            // ├── Identifier
            // ├── Equal
            // ├── Expr
            // └── SemiColon

            var declaration = GetNode(token, SyntaxTokenKind.DeclarationStatement);
            var declKeyword = GetTerminal(declaration.Tokens[0]);
            var nameTerminal = GetTerminal(declaration.Tokens[1], SyntaxTokenKind.Identifier);
            var exprSyntax = GetNode(declaration.Tokens[3]);

            bool isConst = declKeyword.Kind == SyntaxTokenKind.ConstKeyword;

            var expr = BindExpression(exprSyntax);
            var exprType = GetResultType(expr, exprSyntax.Location, mustBeValue: true, mustBeModifiableValue: false, mustBeTypeName: false);

            var varType = exprType;

            var name = nameTerminal.Text;

            Symbol symbol;

            if (InGlobalScope())
                symbol = new GlobalVariableSymbol(name, varType, isConst);
            else
                symbol = new LocalVariableSymbol(name, varType, isConst);

            if (!DeclareSymbol(symbol))
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NameAlreadyExistsError(nameTerminal.Location, name);
            }

            return new BoundDeclarationStatement(symbol, expr);
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
                    return BindLiteral(token);
                case SyntaxTokenKind.Identifier:
                    return BindIdentifier(token);
                default:
                    throw new Exception($"unexpected kind: {token.Kind}");
            }
        }

        private BoundExpression BindAssignmentHelper(SyntaxToken token)
        {
            // AssignmentHelper
            // ├── Expr
            // └── Assignment (optional)
            //     ├── Equal
            //     ├── Expr
            //     └── Assignment (optional)

            var helper = GetNode(token, SyntaxTokenKind.AssignmentHelper);

            if (helper.Tokens[1].Kind != SyntaxTokenKind.Assignment)
                return BindExpression(helper.Tokens[0]);

            return BindAssignment(helper.Tokens[0], helper.Tokens[1]);
        }

        private BoundExpression BindAssignment(SyntaxToken toAssign, SyntaxToken token)
        {
            // Assignment
            // ├── Equal
            // ├── Expr
            // └── Assignment (optional)

            var assignment = GetNode(token, SyntaxTokenKind.Assignment);
            var assignmentLocation = SourceLocation.CreateAround(toAssign.Location, assignment.Location);

            var opSyntax = GetTerminal(assignment.Tokens[0]);
            var exprSyntax = assignment.Tokens[1];

            var left = BindExpression(toAssign);
            var op = SemanticInfo.GetAssignmentOperation(opSyntax.Kind);

            var childAssign = assignment.Tokens[2];
            bool hasChildAssign = childAssign.Kind == SyntaxTokenKind.Assignment;

            var rightLocation = hasChildAssign ? SourceLocation.CreateAround(exprSyntax.Location, childAssign.Location) : exprSyntax.Location;

            BoundExpression right;
            if (childAssign.Kind == SyntaxTokenKind.Assignment)
                right = BindAssignment(exprSyntax, childAssign);
            else
                right = BindExpression(exprSyntax);


            var leftType = GetResultType(left, toAssign.Location, mustBeValue: true, mustBeModifiableValue: true, mustBeTypeName: false);
            var rightType = GetResultType(right, rightLocation, mustBeValue: true, mustBeModifiableValue: false, mustBeTypeName: false);

            if (op == AssignmentOperation.Assign)
            {
                if (IsTypeMissmatch(leftType, rightType))
                    diagnostics.MissmatchedTypeError(assignmentLocation, leftType, rightType);
            }
            else
            {
                var binaryOperation = SemanticInfo.GetOperationForAssignment(op);
                var binaryOperationResult = GetBinaryOperationResult(left, binaryOperation, right);

                if (binaryOperationResult is null)
                    diagnostics.UnsupportedAssignmentOperation(assignmentLocation, op, leftType, rightType);
                else if (IsTypeMissmatch(leftType, binaryOperationResult))
                    diagnostics.MissmatchedTypeError(assignmentLocation, leftType, binaryOperationResult);
            }

            return new BoundAssignmentExpression(typeSystem, left, op, right);
        }

        private BoundExpression BindBinary(SyntaxToken token)
        {
            // Binary
            // ├── Expr
            // ├── Operator
            // └── Expr

            var binary = GetNode(token);

            var leftSyntax = binary.Tokens[0];
            var opTerminal = GetTerminal(binary.Tokens[1]);
            var rightSyntax = binary.Tokens[2];

            var left = BindExpression(leftSyntax);
            var op = SemanticInfo.GetBinaryOperation(opTerminal.Kind);
            var right = BindExpression(rightSyntax);

            var resultType = GetBinaryOperationResult(left, op, right);

            if (resultType is not null)
                return new BoundBinaryExpression(left, op, right, resultType);

            if (!opTerminal.Invalid)
                diagnostics.UnsupportedBinaryOperation(token.Location, op, left.ResultType, right.ResultType);
            return new BoundBinaryExpression(left, op, right, typeSystem.Error);
        }

        private BoundExpression BindPrefix(SyntaxToken token)
        {
            // Prefix
            // ├── Operator
            // └── Expr

            var prefix = GetNode(token, SyntaxTokenKind.Prefix);

            var opTerminal = GetTerminal(prefix.Tokens[0]);
            var exprSyntax = prefix.Tokens[1];

            var op = SemanticInfo.GetUnaryOperation(opTerminal.Kind);
            var expr = BindExpression(exprSyntax);

            var resultType = GetUnaryOperationResult(op, expr);

            if (resultType is not null)
                return new BoundUnaryExpression(expr, op, resultType);

            if (!opTerminal.Invalid)
                diagnostics.UnsupportedUnaryOperation(token.Location, op, expr.ResultType);

            return new BoundUnaryExpression(expr, op, typeSystem.Error);
        }

        private BoundExpression BindPostfix(SyntaxToken token)
        {
            var postfix = GetNode(token);

            switch (postfix.Tokens[1].Kind)
            {
                case SyntaxTokenKind.Dot:
                    return BindMeberAccess(token);
                case SyntaxTokenKind.LeftParen:
                    return BindCallExpression(token);
                case SyntaxTokenKind.LeftSquare:
                    return BindArrayAccess(token);
                default:
                    throw new Exception($"Unexpected token kind: {token.Kind}");
            }
        }

        private BoundExpression BindMeberAccess(SyntaxToken token)
        {
            // Postfix
            // ├── Expr
            // ├── Dot
            // └── Expr

            throw new NotImplementedException();
        }

        private BoundExpression BindCallExpression(SyntaxToken token)
        {
            // Postfix
            // ├── Expr
            // ├── LeftParen
            // ├── Arguments (optional)
            // └── RightParen

            throw new NotImplementedException();
        }

        private BoundExpression BindArrayAccess(SyntaxToken token)
        {
            // Postfix
            // ├── Expr
            // ├── LeftSquare
            // ├── Arguments
            // └── RightSquare

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
            var terminal = GetTerminal(token);
            var type = SemanticInfo.GetTypeFromLiteral(typeSystem, token.Kind);
            var value = GetValueForLiteral(terminal, type);

            if (value is not null)
                return new BoundLiteralExpression(type, value);

            if (!terminal.Invalid)
                diagnostics.InvalidLiteralError(token.Location);
            return new BoundInvalidExpression(typeSystem);
        }

        private object? GetValueForLiteral(SyntaxTerminal token, TypeSpecifier type)
        {
            var text = token.Text;

            switch (token.Kind)
            {
                case SyntaxTokenKind.Int:
                    long.TryParse(text, out var res1);
                    return res1;
                case SyntaxTokenKind.Float:
                    double.TryParse(text, out var res2);
                    return res2;
                case SyntaxTokenKind.Bool:
                    bool.TryParse(text, out var res3);
                    return res3;
                case SyntaxTokenKind.String:
                    return token.Text;
                default:
                    throw new System.Exception($"Unexpected literal type: {token.Kind}");
            }
        }

        private BoundExpression BindIdentifier(SyntaxToken token)
        {
            var nameTerminal = GetTerminal(token, SyntaxTokenKind.Identifier);
            var name = nameTerminal.Text;
            var symbol = LookupSymbol(name);

            if (symbol is not null)
                return new BoundSymbolExpression(symbol);

            if (symbol is not ErrorSymbol && !nameTerminal.Invalid)
                diagnostics.NonExistantNameError(nameTerminal.Location, name);

            return new BoundInvalidExpression(typeSystem);
        }

        private TypeSpecifier? GetBinaryOperationResult(BoundExpression left, BoundOperation op, BoundExpression right)
        {
            Debug.Assert(op.IsBinaryOperation());

            var leftType = left.ResultType;
            var rightType = right.ResultType;

            if (leftType is ErrorType || rightType is ErrorType)
                return typeSystem.Error;

            if (!left.IsValue || !right.IsValue)
                return null;

            var name = SemanticInfo.GetFunctionNameFromOperation(op);

            var methodGroup = (leftType.ReadOnlyScope?.LookupSymbol(name) as OperationSymbol)?.MethodGroup;

            if (methodGroup is null)
                return null;

            var args = new[] { leftType, rightType };

            var method = FindMethodOverload(methodGroup, args, true);

            return method?.ReturnType;
        }

        private TypeSpecifier? GetUnaryOperationResult(BoundOperation op, BoundExpression expr)
        {
            Debug.Assert(op.IsUnaryOperation());

            var type = expr.ResultType;

            if (type is ErrorType)
                return typeSystem.Error;

            if (!expr.IsValue)
                return null;

            var name = SemanticInfo.GetFunctionNameFromOperation(op);

            var methodGroup = (type.ReadOnlyScope?.LookupSymbol(name) as OperationSymbol)?.MethodGroup;

            if (methodGroup is null)
                return null;

            var args = new[] { type };

            var method = FindMethodOverload(methodGroup, args, true);

            return method?.ReturnType;
        }
    }
}