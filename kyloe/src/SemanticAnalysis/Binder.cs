using System;
using Kyloe.Syntax;
using Kyloe.Symbols;

using System.Diagnostics;
using Kyloe.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Semantics
{
    internal class LocalVariableScopeStack
    {
        private readonly Stack<LocalVariableScope> scopes;

        public LocalVariableScopeStack()
        {
            scopes = new Stack<LocalVariableScope>();
            scopes.Push(new LocalVariableScope());
        }

        public void EnterNewScope()
        {
            scopes.Push(new LocalVariableScope());
        }

        public void ExitCurrentScope()
        {
            scopes.Pop();
        }

        public bool TryDeclareLocal(string name, ITypeSymbol type)
        {
            return scopes.Peek().TryDeclareLocal(name, type);
        }

        public ILocalVariableSymbol? LookupLocal(string name)
        {
            foreach (var scope in scopes)
                if (scope.TryGetLocal(name) is ILocalVariableSymbol symbol)
                    return symbol;

            return null;
        }
    }


    internal class Binder
    {
        private readonly TypeSystem typeSystem;
        private readonly DiagnosticCollector diagnostics;
        private readonly LocalVariableScopeStack locals;

        public Binder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
        {
            this.typeSystem = typeSystem;
            this.diagnostics = diagnostics;
            this.locals = new LocalVariableScopeStack();
        }

        private BoundExpression BindExpressionAndExpectValue(SyntaxExpression expr, out ITypeSymbol result)
        {
            var bound = BindExpression(expr);

            if (bound.ResultSymbol is IErrorTypeSymbol)
            {
                result = typeSystem.Error;
                return bound;
            }

            if (bound.IsValue && bound.ResultSymbol is ITypeSymbol typeSymbol)
            {
                result = typeSymbol;
                return bound;
            }

            diagnostics.Add(new ExpectedValueError(expr));

            result = typeSystem.Error;
            return bound;
        }

        private BoundExpression BindExpressionAndExpectType(SyntaxExpression expr, ITypeSymbol expectedType)
        {
            var bound = BindExpression(expr);

            if (bound.ResultSymbol is IErrorTypeSymbol)
                return bound;

            if (!bound.IsValue)
                diagnostics.Add(new ExpectedValueError(expr));
            else if (bound.ResultSymbol != expectedType)
                diagnostics.Add(new MissmatchedTypeError(expr, expectedType, bound.ResultSymbol));

            return bound;
        }

        public BoundNode Bind(SyntaxNode node)
        {
            if (node is SyntaxStatement statement)
                return BindStatement(statement);
            else if (node is SyntaxExpression expression)
                return BindExpression(expression);
            else throw new Exception($"Unexpected SyntaxNode: {node.GetType()}");
        }

        public BoundStatement BindStatement(SyntaxStatement stmt)
        {
            switch (stmt.Type)
            {
                case SyntaxNodeType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatement)stmt);
                case SyntaxNodeType.DeclarationStatement:
                    return BindDeclarationStatement((DeclarationStatement)stmt);
                case SyntaxNodeType.IfStatement:
                    return BindIfStatement((IfStatement)stmt);
                case SyntaxNodeType.EmptyStatement:
                    return BindEmptyStatement((EmptyStatement)stmt);
                case SyntaxNodeType.BlockStatement:
                    return BindBlockStatement((BlockStatement)stmt);
                default:
                    throw new Exception($"Unexpected SyntaxStatement: {stmt.Type}");
            }
        }

        private BoundStatement BindBlockStatement(BlockStatement stmt)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();

            locals.EnterNewScope();

            foreach (var statement in stmt.Statements)
                builder.Add(BindStatement(statement));

            locals.ExitCurrentScope();

            return new BoundBlockStatement(builder.ToImmutable());
        }

        private BoundStatement BindEmptyStatement(EmptyStatement stmt)
        {
            return new BoundEmptyStatement();
        }

        private BoundStatement BindIfStatement(IfStatement stmt)
        {
            var condition = BindExpressionAndExpectType(stmt.Condition, typeSystem.Bool);

            var body = BindStatement(stmt.Body);
            var elseClasue = stmt.ElseClause == null ? null : BindStatement(stmt.ElseClause.Body);

            return new BoundIfStatement(condition, body, elseClasue);
        }

        private BoundStatement BindDeclarationStatement(DeclarationStatement stmt)
        {
            var expr = BindExpressionAndExpectValue(stmt.AssignmentExpression, out var type);
            string name = ExtractName(stmt.NameToken);

            if (!locals.TryDeclareLocal(name, type))
                diagnostics.Add(new RedefinedLocalVariableError(stmt.NameToken));

            var symbol = locals.LookupLocal(name)!;

            return new BoundDeclarationStatement(symbol, expr);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatement stmt)
        {
            var expr = BindExpression(stmt.Expression);
            return new BoundExpressionStatement(expr);
        }

        public BoundExpression BindExpression(SyntaxExpression expr)
        {
            switch (expr.Type)
            {
                case SyntaxNodeType.MalformedExpression:
                    return BindMalformedExpression((MalformedExpression)expr);
                case SyntaxNodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)expr);
                case SyntaxNodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)expr);
                case SyntaxNodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)expr);
                case SyntaxNodeType.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpression)expr);
                case SyntaxNodeType.NameExpression:
                    return BindNameExpression((NameExpression)expr);
                case SyntaxNodeType.MemberAccessExpression:
                    return BindMemberAccessExpression((MemberAccessExpression)expr);
                case SyntaxNodeType.SubscriptExpression:
                    return BindSubscriptExpression((SubscriptExpression)expr);
                case SyntaxNodeType.CallExpression:
                    return BindCallExpression((CallExpression)expr);
                case SyntaxNodeType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)expr);
                default:
                    throw new System.Exception($"Unexpected SyntaxExpression: {expr.Type}");
            }
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindCallExpression(CallExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindSubscriptExpression(SubscriptExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindMemberAccessExpression(MemberAccessExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindNameExpression(NameExpression expr)
        {
            var name = ExtractName(expr.NameToken);

            var local = locals.LookupLocal(name);

            if (local is null)
            {
                diagnostics.Add(new NonExistantNameError(expr.NameToken));
                return new BoundInvalidExpression(typeSystem);
            }

            return new BoundLocalVariableExpression(local);
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpression expr)
        {
            return BindExpression(expr.Expression);
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expr)
        {
            var left = BindExpression(expr.LeftExpression);
            var right = BindExpression(expr.RightExpression);
            var op = SemanticInfo.GetBinaryOperation(expr.OperatorToken.Type);
            var resultType = GetBinaryOperationResult(left, op, right);

            if (resultType is not null)
                return new BoundBinaryExpression(left, op, right, resultType);

            diagnostics.Add(new UnsupportedBinaryOperation(expr, left.ResultSymbol, right.ResultSymbol));

            return new BoundBinaryExpression(left, op, right, typeSystem.Error);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expr)
        {
            var childExpression = BindExpression(expr.Expression);
            var op = SemanticInfo.GetUnaryOperation(expr.OperatorToken.Type);

            var resultType = GetUnaryOperationResult(op, childExpression);

            if (resultType is not null)
                return new BoundUnaryExpression(childExpression, op, resultType);

            diagnostics.Add(new UnsupportedUnaryOperation(expr, childExpression.ResultSymbol));

            return new BoundUnaryExpression(childExpression, op, typeSystem.Error);
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expr)
        {
            var type = SemanticInfo.GetTypeFromLiteral(typeSystem, expr.LiteralToken.Type);
            var value = expr.LiteralToken.Value;
            Debug.Assert(value is not null, "Literal token should always have a value!");

            return new BoundLiteralExpression(type, value);
        }

        private BoundExpression BindMalformedExpression(MalformedExpression expr)
        {
            return new BoundInvalidExpression(typeSystem);
        }

        private static string ExtractName(SyntaxToken nameToken)
        {
            Debug.Assert(nameToken.Type == SyntaxTokenType.Identifier);
            Debug.Assert(nameToken.Value is string, $"value of name token wasn't a string: value={nameToken.Value}");

            var name = (string)nameToken.Value;
            return name;
        }

        private ITypeSymbol? GetBinaryOperationResult(BoundExpression left, BinaryOperation op, BoundExpression right)
        {
            if (!left.IsValue || !right.IsValue)
                return null;

            if (left.ResultSymbol is IErrorTypeSymbol || right.ResultSymbol is IErrorTypeSymbol)
                return typeSystem.Error;

            if (!(left.ResultSymbol is ITypeSymbol leftType) || !(right.ResultSymbol is ITypeSymbol rightType))
                return null;

            var name = SemanticInfo.GetBinaryOperationMethodName(op);

            if (name is null)
                throw new NotImplementedException();

            var methods = leftType.LookupMembers(name).Where(
                member => member is IMethodSymbol method &&
                method.IsOperator &&
                method.Parameters.Count() == 2 &&
                method.Parameters.First().Type == leftType &&
                method.Parameters.Last().Type == rightType
            );

            if (methods.Count() > 1)
                Console.WriteLine($"Note: Found multiple methods for an operator: op={op}, left={leftType}, right={rightType}");

            var method = methods.FirstOrDefault() as IMethodSymbol;

            if (method is null)
                return null;

            return method.ReturnType;
        }

        private ITypeSymbol? GetUnaryOperationResult(UnaryOperation op, BoundExpression expr)
        {
            if (expr.ResultSymbol is IErrorTypeSymbol)
                return typeSystem.Error;

            if (!(expr.ResultSymbol is ITypeSymbol type))
                return null;

            var name = SemanticInfo.GetUnaryOperationMethodName(op);

            if (name is null)
                throw new NotImplementedException();

            var methods = type.LookupMembers(name).Where(
                member => member is IMethodSymbol method &&
                method.IsOperator &&
                method.Parameters.Count() == 1 &&
                method.Parameters.First().Type == type
            );

            if (methods.Count() > 1)
                Console.WriteLine($"Note: Found multiple methods for a unary operator: op={op}, type={type}");

            var method = methods.FirstOrDefault() as IMethodSymbol;

            if (method is null)
                return null;

            return method.ReturnType;
        }
    }
}