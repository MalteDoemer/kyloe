using System;
using System.Collections.Immutable;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal abstract class LoweredTreeRewriter
    {
        protected readonly TypeSystem typeSystem;

        protected LoweredTreeRewriter(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }


        public virtual LoweredCompilationUnit RewriteCompilationUnit(LoweredCompilationUnit compilationUnit)
        {
            var global = RewriteStatement(compilationUnit.GlobalStatement);

            var builder = ImmutableArray.CreateBuilder<LoweredFunctionDefinition>(compilationUnit.LoweredFunctions.Length);

            foreach (var func in compilationUnit.LoweredFunctions)
                builder.Add(RewriteFunctionDefinition(func));

            return new LoweredCompilationUnit(builder.MoveToImmutable(), global, compilationUnit.MainFunctionIndex);
        }

        protected virtual LoweredFunctionDefinition RewriteFunctionDefinition(LoweredFunctionDefinition functionDefinition)
        {
            var body = RewriteBlockStatement(functionDefinition.Body);

            if (body == functionDefinition.Body)
                return functionDefinition;

            return new LoweredFunctionDefinition(functionDefinition.Type, body);
        }

        protected virtual LoweredStatement RewriteStatement(LoweredStatement statement)
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
                case LoweredNodeKind.LoweredGotoStatement:
                    return RewriteGotoStatement((LoweredGotoStatement)statement);
                case LoweredNodeKind.LoweredConditionalGotoStatement:
                    return RewriteConditionalGotoStatement((LoweredConditionalGotoStatement)statement);
                case LoweredNodeKind.LoweredLabelStatement:
                    return RewriteLabelStatement((LoweredLabelStatement)statement);

                default:
                    throw new Exception($"unexpected kind: {statement.Kind}");
            }
        }

        protected virtual LoweredStatement RewriteGotoStatement(LoweredGotoStatement statement)
        {
            return statement;
        }

        protected virtual LoweredStatement RewriteConditionalGotoStatement(LoweredConditionalGotoStatement statement)
        {
            var expr = RewriteExpression(statement.Condition);

            if (expr == statement.Condition)
                return statement;

            return new LoweredConditionalGotoStatement(statement.Label, expr);
        }

        protected virtual LoweredStatement RewriteLabelStatement(LoweredLabelStatement statement)
        {
            return statement;
        }

        protected virtual LoweredBlockStatement RewriteBlockStatement(LoweredBlockStatement statement)
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

            if (expr == statement.Expression)
                return statement;

            return new LoweredReturnStatement(expr);
        }

        protected virtual LoweredStatement RewriteWhileStatement(LoweredWhileStatement statement)
        {
            var condition = RewriteExpression(statement.Condition);
            var body = RewriteStatement(statement.Body);

            if (condition == statement.Condition && body == statement.Body)
                return statement;

            return new LoweredWhileStatement(statement.BreakLabel, statement.ContinueLabel, condition, body);
        }

        protected virtual LoweredStatement RewriteExpressionStatement(LoweredExpressionStatement statement)
        {
            var expr = RewriteExpression(statement.Expression);

            if (expr == statement.Expression)
                return statement;

            return new LoweredExpressionStatement(expr);
        }

        protected virtual LoweredStatement RewriteDeclarationStatement(LoweredDeclarationStatement statement)
        {
            if (statement.Initializer is null)
                return statement;

            var initializer = RewriteExpression(statement.Initializer);

            if (initializer == statement.Initializer)
                return statement;

            return new LoweredDeclarationStatement(statement.Symbol, initializer);
        }

        protected virtual LoweredStatement RewriteEmptyStatement(LoweredEmptyStatement statement)
        {
            return statement;
        }

        protected virtual LoweredStatement RewriteIfStatement(LoweredIfStatement statement)
        {
            var condition = RewriteExpression(statement.Condition);
            var body = RewriteStatement(statement.Body);
            var elsebody = RewriteStatement(statement.ElseStatment);

            if (condition == statement.Condition && body == statement.Body && elsebody == statement.ElseStatment)
                return statement;

            return new LoweredIfStatement(condition, body, elsebody);
        }

        protected virtual LoweredExpression RewriteExpression(LoweredExpression expression)
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
                case LoweredNodeKind.LoweredSymbolExpression:
                    return RewriteVariableAccessExpression((LoweredSymbolExpression)expression);
                case LoweredNodeKind.LoweredCallExpression:
                    return RewriteCallExpression((LoweredCallExpression)expression);
                default:
                    throw new Exception($"unexpected kind: {expression.Kind}");
            }
        }

        protected virtual LoweredExpression RewriteCallExpression(LoweredCallExpression expression)
        {
            var expr = RewriteExpression(expression.Expression);
            var args = RewriteArguments(expression.Arguments);

            if (expr == expression.Expression && args == expression.Arguments)
                return expression;

            return new LoweredCallExpression(expression.CallableType, expr, args);
        }

        protected virtual LoweredArguments RewriteArguments(LoweredArguments arguments)
        {
            var builder = ImmutableArray.CreateBuilder<LoweredExpression>(arguments.Arguments.Length);

            foreach (var arg in arguments)
                builder.Add(RewriteExpression(arg));

            return new LoweredArguments(builder.MoveToImmutable());
        }

        protected virtual LoweredExpression RewriteLiteralExpression(LoweredLiteralExpression expression)
        {
            return expression;
        }

        protected virtual LoweredExpression RewriteBinaryExpression(LoweredBinaryExpression expression)
        {
            var left = RewriteExpression(expression.LeftExpression);
            var right = RewriteExpression(expression.RightExpression);

            if (left == expression.LeftExpression && right == expression.RightExpression)
                return expression;

            return new LoweredBinaryExpression(expression.Type, left, expression.Operation, right);
        }

        protected virtual LoweredExpression RewriteUnaryExpression(LoweredUnaryExpression expression)
        {
            var expr = RewriteExpression(expression.Expression);

            if (expr == expression.Expression)
                return expression;

            return new LoweredUnaryExpression(expression.Type, expr, expression.Operation);
        }

        protected virtual LoweredExpression RewriteAssignment(LoweredAssignment expression)
        {
            var left = RewriteExpression(expression.LeftExpression);
            var right = RewriteExpression(expression.RightExpression);

            if (left == expression.LeftExpression && right == expression.RightExpression)
                return expression;

            return new LoweredAssignment(typeSystem, left, expression.Operation, right);
        }

        protected virtual LoweredExpression RewriteVariableAccessExpression(LoweredSymbolExpression expression)
        {
            return expression;
        }

    }
}