using System;
using System.Collections.Immutable;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal abstract class LoweredTreeRewriter
    {
        private readonly TypeSystem typeSystem;

        protected LoweredTreeRewriter(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public virtual LoweredStatement RewriteStatement(LoweredStatement statement)
        {
            switch (statement.Kind)
            {
                case LoweredNodeKind.LoweredBlockStatement:
                    return RewriteBlockStatement((LoweredBlockStatement)statement);
                case LoweredNodeKind.LoweredContinueStatement:
                    return RewriteContinueStatement((LoweredContinueStatement)statement);
                case LoweredNodeKind.LoweredBreakStatement:
                    return RewriteBreakStatement((LoweredBreakStatement)statement);
                case LoweredNodeKind.LoweredReturnStatement:
                    return RewriteReturnStatement((LoweredReturnStatement)statement);
                case LoweredNodeKind.LoweredWhileStatement:
                    return RewriteWhileStatement((LoweredWhileStatement)statement);
                case LoweredNodeKind.LoweredExpressionStatement:
                    return RewriteExpressionStatement((LoweredExpressionStatement)statement);
                case LoweredNodeKind.LoweredDeclarationStatement:
                    return RewriteDeclarationStatement((LoweredDeclarationStatement)statement);
                case LoweredNodeKind.LoweredEmptyStatement:
                    return RewriteEmptyStatement((LoweredEmptyStatement)statement);
                case LoweredNodeKind.LoweredIfStatement:
                    return RewriteIfStatement((LoweredIfStatement)statement);

                default:
                    throw new Exception($"unexpected kind: {statement.Kind}");
            }
        }

        protected virtual LoweredStatement RewriteBlockStatement(LoweredBlockStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<LoweredStatement>(statement.Statements.Length);

            foreach (var stmt in statement)
                builder.Add(RewriteStatement(stmt));

            return new LoweredBlockStatement(builder.MoveToImmutable());
        }

        protected virtual LoweredStatement RewriteContinueStatement(LoweredContinueStatement statement)
        {
            return statement;
        }

        protected virtual LoweredStatement RewriteBreakStatement(LoweredBreakStatement statement)
        {
            return statement;
        }

        protected virtual LoweredStatement RewriteReturnStatement(LoweredReturnStatement statement)
        {
            if (statement.Expression is null)
                return statement;

            var expr = RewriteExpression(statement.Expression);
            return new LoweredReturnStatement(expr);
        }

        protected virtual LoweredStatement RewriteWhileStatement(LoweredWhileStatement statement)
        {
            var condition = RewriteExpression(statement.Condition);
            var body = RewriteStatement(statement.Body);
            return new LoweredWhileStatement(statement.BreakLabel, statement.ContinueLabel, condition, body);
        }

        protected virtual LoweredStatement RewriteExpressionStatement(LoweredExpressionStatement statement)
        {
            var expr = RewriteExpression(statement.Expression);
            return new LoweredExpressionStatement(expr);
        }

        protected virtual LoweredStatement RewriteDeclarationStatement(LoweredDeclarationStatement statement)
        {
            if (statement.Initializer is null)
                return statement;

            var initializer = RewriteExpression(statement.Initializer);
            return new LoweredDeclarationStatement(statement.Symbol, initializer);
        }

        private LoweredStatement RewriteEmptyStatement(LoweredEmptyStatement statement)
        {
            return statement;
        }

        private LoweredStatement RewriteIfStatement(LoweredIfStatement statement)
        {
            var condition = RewriteExpression(statement.Condition);
            var body = RewriteStatement(statement.Body);
            var elsebody = RewriteStatement(statement.ElseStatment);

            return new LoweredIfStatement(condition, body, elsebody);
        }

        public LoweredExpression RewriteExpression(LoweredExpression expression)
        {
            switch (expression.Kind)
            {
                case LoweredNodeKind.LoweredLiteralExpression:
                    return RewriteLiteralExpression((LoweredLiteralExpression)expression);
                case LoweredNodeKind.LoweredBinaryExpression:
                    return RewriteBinaryExpression((LoweredBinaryExpression)expression);
                case LoweredNodeKind.LoweredUnaryExpression:
                    return RewriteUnaryExpression((LoweredUnaryExpression)expression);
                case LoweredNodeKind.LoweredAssignment:
                    return RewriteAssignment((LoweredAssignment)expression);
                case LoweredNodeKind.LoweredVariableAccessExpression:
                    return RewriteVariableAccessExpression((LoweredVariableAccessExpression)expression);

                default:
                    throw new Exception($"unexpected kind: {expression.Kind}");
            }
        }

        protected virtual LoweredExpression RewriteLiteralExpression(LoweredLiteralExpression expression)
        {
            return expression;
        }

        protected virtual LoweredExpression RewriteBinaryExpression(LoweredBinaryExpression expression)
        {
            var left = RewriteExpression(expression.LeftExpression);
            var right = RewriteExpression(expression.RightExpression);
            return new LoweredBinaryExpression(expression.Type, left, expression.Operation, right);
        }

        protected virtual LoweredExpression RewriteUnaryExpression(LoweredUnaryExpression expression)
        {
            var expr = RewriteExpression(expression.Expression);
            return new LoweredUnaryExpression(expression.Type, expr, expression.Operation);
        }

        protected virtual LoweredExpression RewriteAssignment(LoweredAssignment expression)
        {
            var left = RewriteExpression(expression.LeftExpression);
            var right = RewriteExpression(expression.RightExpression);
            return new LoweredAssignment(typeSystem, left, expression.Operation, right);
        }

        protected virtual LoweredExpression RewriteVariableAccessExpression(LoweredVariableAccessExpression expression)
        {
            return expression;
        }
    }
}