using System;
using Kyloe.Syntax;
using Kyloe.Symbols;

using System.Diagnostics;
using Kyloe.Diagnostics;
using System.Collections.Immutable;

namespace Kyloe.Semantics
{
    internal class Binder
    {
        private readonly TypeSystem typeSystem;
        private readonly DiagnosticCollector diagnostics;

        public Binder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
        {
            this.typeSystem = typeSystem;
            this.diagnostics = diagnostics;
        }

        private BoundResultType ExpectValue(SyntaxExpression original, BoundResultType result)
        {
            if (result.IsValue)
                return result;

            if (!result.IsError)
                diagnostics.Add(new ExpectedValueError(original));

            return BoundResultType.ErrorResult;
        }

        private BoundResultType ExpectValueType(SyntaxExpression original, BoundResultType result, ITypeSymbol type)
        {
            var res = ExpectValue(original, result);

            if (res.IsError)
                return res;

            if (res.Symbol == type)
                return res;


            diagnostics.Add(new MissmatchedTypeError(original, type, res.Symbol));

            return BoundResultType.ErrorResult;
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

            foreach (var statement in stmt.Statements)
                builder.Add(BindStatement(statement));

            return new BoundBlockStatement(builder.ToImmutable());
        }

        private BoundStatement BindEmptyStatement(EmptyStatement stmt)
        {
            return new BoundEmptyStatement();
        }

        private BoundStatement BindIfStatement(IfStatement stmt)
        {
            var condition = BindExpression(stmt.Condition);
            ExpectValueType(stmt.Condition, condition.Result, typeSystem.Bool);

            var body = BindStatement(stmt.Body);
            var elseClasue = stmt.ElseClause == null ? null : BindStatement(stmt.ElseClause.Body);

            return new BoundIfStatement(condition, body, elseClasue);

        }

        private BoundStatement BindDeclarationStatement(DeclarationStatement stmt)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

            var leftType = ExpectValue(expr.LeftExpression, left.Result);
            var rightType = ExpectValue(expr.RightExpression, right.Result);

            var resultType = SemanticInfo.GetBinaryOperationResult(leftType, op, rightType);

            if (resultType is not null)
                return new BoundBinaryExpression(left, op, right, resultType);

            diagnostics.Add(new UnsupportedBinaryOperation(expr, leftType, rightType));

            return new BoundBinaryExpression(left, op, right, BoundResultType.ErrorResult);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expr)
        {
            var childExpression = BindExpression(expr.Expression);
            var op = SemanticInfo.GetUnaryOperation(expr.OperatorToken.Type);

            var type = ExpectValue(expr.Expression, childExpression.Result);
            var resultType = SemanticInfo.GetUnaryOperationResult(op, type);

            if (resultType is not null)
                return new BoundUnaryExpression(childExpression, op, resultType);

            diagnostics.Add(new UnsupportedUnaryOperation(expr, type));

            return new BoundUnaryExpression(childExpression, op, BoundResultType.ErrorResult);
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
            return new BoundInvalidExpression();
        }
    }
}