using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal class Lowerer
    {

        private readonly TypeSystem typeSystem;

        private readonly Stack<(LoweredLabel breakLabel, LoweredLabel continueLabel)> loopLableStack;

        public Lowerer(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            this.loopLableStack = new Stack<(LoweredLabel, LoweredLabel)>();
        }

        public LoweredCompilationUnit LowerCompilationUnit(BoundCompilationUnit compilationUnit)
        {
            var globals = ImmutableArray.CreateBuilder<LoweredStatement>(compilationUnit.Globals.Length);
            var functions = ImmutableArray.CreateBuilder<LoweredFunctionDefinition>(compilationUnit.Functions.Length);

            var mainIndex = -1;

            foreach (var (i, function) in compilationUnit.Functions.EnumerateIndex())
            {
                if (object.ReferenceEquals(function, compilationUnit.MainFunction))
                    mainIndex = i;

                functions.Add(LowerFunctionDefinition(function));
            }

            foreach (var global in compilationUnit.Globals)
                globals.Add(LowerStatement(global));

            var globalStatement = new LoweredBlockStatement(globals.MoveToImmutable());

            return new LoweredCompilationUnit(functions.MoveToImmutable(), globalStatement, mainIndex);
        }

        private LoweredFunctionDefinition LowerFunctionDefinition(BoundFunctionDefinition functionDefinition)
        {
            var body = LowerBlockStatement(functionDefinition.Body);
            return new LoweredFunctionDefinition(functionDefinition.FunctionType, body);
        }

        private LoweredStatement LowerStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.BoundBlockStatement:
                    return LowerBlockStatement((BoundBlockStatement)statement);
                case BoundNodeKind.BoundEmptyStatement:
                    return LowerEmptyStatement((BoundEmptyStatement)statement);
                case BoundNodeKind.BoundIfStatement:
                    return LowerIfStatement((BoundIfStatement)statement);
                case BoundNodeKind.BoundDeclarationStatement:
                    return LowerDeclarationStatement((BoundDeclarationStatement)statement);
                case BoundNodeKind.BoundExpressionStatement:
                    return LowerExpressionStatement((BoundExpressionStatement)statement);
                case BoundNodeKind.BoundWhileStatement:
                    return LowerWhileStatement((BoundWhileStatement)statement);
                case BoundNodeKind.BoundForStatement:
                    return LowerForStatement((BoundForStatement)statement);
                case BoundNodeKind.BoundReturnStatement:
                    return LowerReturnStatement((BoundReturnStatement)statement);
                case BoundNodeKind.BoundContinueStatement:
                    return LowerContinueStatement((BoundContinueStatement)statement);
                case BoundNodeKind.BoundBreakStatement:
                    return LowerBreakStatement((BoundBreakStatement)statement);

                default:
                    throw new Exception($"unexpected kind: {statement.Kind}");
            }
        }

        private LoweredBlockStatement LowerBlockStatement(BoundBlockStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<LoweredStatement>(statement.Statements.Length);

            foreach (var stmt in statement.Statements)
                builder.Add(LowerStatement(stmt));

            return new LoweredBlockStatement(builder.MoveToImmutable());
        }

        private LoweredStatement LowerEmptyStatement(BoundEmptyStatement statement)
        {
            return new LoweredEmptyStatement();
        }

        private LoweredStatement LowerIfStatement(BoundIfStatement statement)
        {
            var condition = LowerExpression(statement.Condition);
            var body = LowerStatement(statement.Body);
            var elseBody = LowerStatement(statement.ElifStatement);

            return new LoweredIfStatement(condition, body, elseBody);
        }

        private LoweredStatement LowerDeclarationStatement(BoundDeclarationStatement statement)
        {
            var expr = LowerExpression(statement.InitializationExpression);
            return new LoweredDeclarationStatement(statement.VariableSymbol, expr);
        }

        private LoweredStatement LowerExpressionStatement(BoundExpressionStatement statement)
        {
            var expr = LowerExpression(statement.Expression);
            return new LoweredExpressionStatement(expr);
        }

        private LoweredStatement LowerWhileStatement(BoundWhileStatement statement)
        {
            var lables = (LoweredLabel.Create("break"), LoweredLabel.Create("continue"));

            var condition = LowerExpression(statement.Condition);

            loopLableStack.Push(lables);
            var body = LowerStatement(statement.Body);
            loopLableStack.Pop();

            return new LoweredWhileStatement(lables.Item1, lables.Item2, condition, body);
        }

        private LoweredStatement LowerForStatement(BoundForStatement statement)
        {
            var lables = (LoweredLabel.Create("break"), LoweredLabel.Create("continue"));

            var decl = LowerStatement(statement.DeclarationStatement);
            var condition = LowerExpression(statement.Condition);
            var increment = LowerExpression(statement.Increment);

            loopLableStack.Push(lables);
            var body = LowerStatement(statement.Body);
            loopLableStack.Pop();

            return new LoweredForStatement(lables.Item1, lables.Item2, decl, condition, increment, body);
        }

        private LoweredStatement LowerReturnStatement(BoundReturnStatement statement)
        {
            var lowered = statement.Expression is not null ? LowerExpression(statement.Expression) : null;
            return new LoweredReturnStatement(lowered);
        }

        private LoweredStatement LowerContinueStatement(BoundContinueStatement statement)
        {
            var labels = loopLableStack.Peek();
            return new LoweredBreakStatement(labels.breakLabel);
        }

        private LoweredStatement LowerBreakStatement(BoundBreakStatement statement)
        {
            var labels = loopLableStack.Peek();
            return new LoweredContinueStatement(labels.continueLabel);
        }

        private LoweredExpression LowerExpression(BoundExpression expression)
        {
            switch (expression.Kind)
            {
                case BoundNodeKind.BoundLiteralExpression:
                    return LowerLiteralExpression((BoundLiteralExpression)expression);
                case BoundNodeKind.BoundBinaryExpression:
                    return LowerBinaryExpression((BoundBinaryExpression)expression);
                case BoundNodeKind.BoundParenthesizedExpression:
                    return LowerParenthesizedExpression((BoundParenthesizedExpression)expression);
                case BoundNodeKind.BoundUnaryExpression:
                    return LowerUnaryExpression((BoundUnaryExpression)expression);
                case BoundNodeKind.BoundAssignmentExpression:
                    return LowerAssignmentExpression((BoundAssignmentExpression)expression);
                case BoundNodeKind.BoundSymbolExpression:
                    return LowerSymbolExpression((BoundSymbolExpression)expression);
                case BoundNodeKind.BoundCallExpression:
                    return LowerCallExpression((BoundCallExpression)expression);
                case BoundNodeKind.BoundConversionExpression:
                    return LowerConversionExpression((BoundConversionExpression)expression);
                default:
                    throw new Exception($"unexpected kind: {expression.Kind}");
            }
        }

        private LoweredExpression LowerConversionExpression(BoundConversionExpression expression)
        {
            var expr = LowerExpression(expression.Expression);
            return new LoweredConversionExpression(expr, expression.Method);
        }

        private LoweredExpression LowerCallExpression(BoundCallExpression expression)
        {
            var expr = LowerExpression(expression.Expression);
            var args = LowerArguments(expression.Arguments);
            return new LoweredCallExpression((CallableType)expression.FunctionType, expr, args);
        }

        private LoweredArguments LowerArguments(BoundArguments arguments)
        {
            var builder = ImmutableArray.CreateBuilder<LoweredExpression>(arguments.Arguments.Length);

            foreach (var arg in arguments)
                builder.Add(LowerExpression(arg));

            return new LoweredArguments(builder.MoveToImmutable());
        }

        private LoweredExpression LowerSymbolExpression(BoundSymbolExpression expression)
        {
            return new LoweredSymbolExpression(expression.Symbol);
        }

        private LoweredExpression LowerAssignmentExpression(BoundAssignmentExpression expression)
        {
            var left = LowerExpression(expression.LeftExpression);
            var right = LowerExpression(expression.RightExpression);

            if (expression.Operation != AssignmentOperation.Assign)
                Debug.Assert(expression.Method is not null);

            return new LoweredAssignment(typeSystem, left, expression.Operation, right, expression.Method);
        }

        private LoweredExpression LowerUnaryExpression(BoundUnaryExpression expression)
        {
            var lowered = LowerExpression(expression.Expression);

            Debug.Assert(expression.Method is not null);

            return new LoweredUnaryExpression(lowered, expression.Operation, expression.Method!);
        }

        private LoweredExpression LowerParenthesizedExpression(BoundParenthesizedExpression expression)
        {
            return LowerExpression(expression.Expression);
        }

        private LoweredExpression LowerBinaryExpression(BoundBinaryExpression expression)
        {
            var left = LowerExpression(expression.LeftExpression);
            var right = LowerExpression(expression.RightExpression);

            Debug.Assert(expression.Method is not null);

            return new LoweredBinaryExpression(left, expression.Operation, right, expression.Method!);
        }

        private LoweredExpression LowerLiteralExpression(BoundLiteralExpression expression)
        {
            return new LoweredLiteralExpression(expression.ResultType, expression.Value);
        }
    }
}
