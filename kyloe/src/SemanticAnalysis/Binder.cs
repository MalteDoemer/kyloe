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

        private (BoundExpression, TypeSpecifier) BindAndExpect(SyntaxExpression expr, bool mustBeValue, bool mustBeModifiable, TypeSpecifier? expectedType = null)
        {
            var bound = BindExpression(expr);

            if (bound.ResultType is ErrorType)
                return (bound, typeSystem.Error);

            if (mustBeValue && !bound.IsValue)
            {
                diagnostics.Add(new ExpectedValueError(expr));
                return (bound, typeSystem.Error);
            }

            if (mustBeModifiable && !bound.IsModifiableValue)
            {
                diagnostics.Add(new ExpectedModifiableValueError(expr));
                return (bound, typeSystem.Error);
            }

            if (expectedType is not null && IsTypeMissmatch(expectedType, bound.ResultType))
            {
                diagnostics.Add(new MissmatchedTypeError(expr, expectedType!, bound.ResultType));
                return (bound, typeSystem.Error);
            }

            return (bound, bound.ResultType);
        }

        public BoundNode Bind(SyntaxNode node)
        {
            if (node is SyntaxStatement statement)
                return BindStatement(statement);
            else if (node is SyntaxExpression expression)
                return BindExpression(expression);
            else if (node is CompilationUnitSyntax compilationUnit)
                return BindCompilationUnit(compilationUnit);
            else throw new Exception($"Unexpected SyntaxNode: {node.GetType()}");
        }

        private BoundNode BindCompilationUnit(CompilationUnitSyntax compilationUnit)
        {
            var functionTypes = new List<FunctionType>(compilationUnit.FunctionDefinitions.Length);
            var globals = ImmutableArray.CreateBuilder<BoundDeclarationStatement>(compilationUnit.GlobalDeclarations.Length);
            var functions = ImmutableArray.CreateBuilder<BoundFunctionDefinition>(compilationUnit.FunctionDefinitions.Length);

            // declare all functions, but not bind the bodies yet
            foreach (var func in compilationUnit.FunctionDefinitions)
                functionTypes.Add(BindFunctionDeclaration(func));

            foreach (var global in compilationUnit.GlobalDeclarations)
                globals.Add(BindDeclarationStatement(global));

            foreach (var (funcType, funcDef) in functionTypes.Zip(compilationUnit.FunctionDefinitions))
                functions.Add(BindFunctionDefinition(funcDef, funcType));

            return new BoundCompilationUnit(globals.MoveToImmutable(), functions.MoveToImmutable());
        }

        private BoundFunctionDefinition BindFunctionDefinition(FunctionDefinition funcDef, FunctionType funcType)
        {
            EnterNewScope(); // this scope contains the parameters

            foreach (var param in funcType.Parameters)
                if (!DeclareSymbol(param))
                    diagnostics.Add(new RedefinedParameterError(funcDef.NameToken));

            var body = BindBlockStatement(funcDef.Body);

            ExitCurrentScope();

            return new BoundFunctionDefinition(funcType, body);
        }

        private FunctionType BindFunctionDeclaration(FunctionDefinition declaration)
        {
            var name = ExtractName(declaration.NameToken);

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
                    diagnostics.Add(new NameAlreadyExistsError(declaration.NameToken));
                functionGroup = new FunctionGroupSymbol(new FunctionGroupType(name));
            }

            var returnType = declaration.TypeClause is null
                             ? typeSystem.Void
                             : BindTypeClause(declaration.TypeClause).Type;

            var function = new FunctionType(name, functionGroup.Group, returnType);


            foreach (var param in declaration.ParameterList.Parameters)
                function.Parameters.Add(BindParameterDeclaration(param));


            bool alreadyExists = false;

            foreach (var otherFunction in functionGroup.Group.Functions)
            {
                if (TypeSequenceEquals(function.Parameters.Select(param => param.Type), otherFunction.Parameters.Select(param => param.Type)))
                {
                    if (symbol is not ErrorSymbol)
                        diagnostics.Add(new OverloadWithSameParametersExistsError(declaration.NameToken));
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                functionGroup.Group.Functions.Add(function);

            return function;
        }

        private ParameterSymbol BindParameterDeclaration(ParameterDeclaration declaration)
        {
            var name = ExtractName(declaration.NameToken);
            var type = BindTypeClause(declaration.TypeClause).Type;
            return new ParameterSymbol(name, type);
        }

        private BoundTypeClause BindTypeClause(TypeClause typeClause)
        {
            var bound = BindExpression(typeClause.NameExpression);

            if (bound.IsTypeName)
                return new BoundTypeClause(bound, bound.ResultType);

            if (bound.ResultType is not ErrorType)
                diagnostics.Add(new ExpectedTypeNameError(typeClause.NameExpression));
            return new BoundTypeClause(bound, typeSystem.Error);
        }

        private BoundStatement BindStatement(SyntaxStatement stmt)
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

        private BoundBlockStatement BindBlockStatement(BlockStatement stmt)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();

            EnterNewScope();

            foreach (var statement in stmt.Statements)
                builder.Add(BindStatement(statement));

            ExitCurrentScope();

            return new BoundBlockStatement(builder.ToImmutable());
        }

        private BoundStatement BindEmptyStatement(EmptyStatement stmt)
        {
            return new BoundEmptyStatement();
        }

        private BoundStatement BindIfStatement(IfStatement stmt)
        {
            var (condition, _) = BindAndExpect(stmt.Condition, mustBeValue: true, mustBeModifiable: false, typeSystem.Bool);

            var body = BindStatement(stmt.Body);
            var elseClasue = stmt.ElseClause == null ? null : BindStatement(stmt.ElseClause.Body);

            return new BoundIfStatement(condition, body, elseClasue);
        }

        private BoundDeclarationStatement BindDeclarationStatement(DeclarationStatement stmt)
        {
            bool isConst = stmt.DeclerationToken.Type == SyntaxTokenType.ConstKeyword;

            var (expr, exprType) = BindAndExpect(stmt.AssignmentExpression, mustBeValue: true, mustBeModifiable: false);

            TypeSpecifier varType;
            BoundTypeClause? typeClause = null;

            if (stmt.TypeClause is not null)
            {
                typeClause = BindTypeClause(stmt.TypeClause);
                varType = typeClause.Type;

                if (IsTypeMissmatch(varType, exprType))
                {
                    diagnostics.Add(new MissmatchedTypeError(stmt.AssignmentExpression, varType, exprType));
                    varType = typeSystem.Error;
                }
            }
            else
            {
                varType = exprType;
            }

            string name = ExtractName(stmt.NameToken);


            Symbol symbol;

            if (InGlobalScope())
                symbol = new GlobalVariableSymbol(name, varType, isConst);
            else
                symbol = new LocalVariableSymbol(name, varType, isConst);

            if (!DeclareSymbol(symbol))
            {

                diagnostics.Add(new NameAlreadyExistsError(stmt.NameToken));
            }

            return new BoundDeclarationStatement(symbol, typeClause, expr);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatement stmt)
        {
            var expr = BindExpression(stmt.Expression);
            return new BoundExpressionStatement(expr);
        }

        private BoundExpression BindExpression(SyntaxExpression expr)
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
                case SyntaxNodeType.IdentifierExpression:
                    return BindIdentifierExpression((IdentifierExpression)expr);
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
            var (left, leftType) = BindAndExpect(expr.LeftNode, mustBeValue: true, mustBeModifiable: true);
            var (right, rightType) = BindAndExpect(expr.RightNode, mustBeValue: true, mustBeModifiable: false);

            var operation = SemanticInfo.GetAssignmentOperation(expr.OperatorToken.Type);

            if (operation == AssignmentOperation.Assign)
            {
                if (IsTypeMissmatch(leftType, rightType))
                    diagnostics.Add(new MissmatchedTypeError(expr, leftType, rightType));
            }
            else
            {
                var binaryOperation = SemanticInfo.GetOperationForAssignment(operation);
                var binaryOperationResult = GetBinaryOperationResult(left, binaryOperation, right);

                if (binaryOperationResult is null)
                    diagnostics.Add(new UnsupportedAssignmentOperation(expr, leftType, rightType));
                else if (IsTypeMissmatch(leftType, binaryOperationResult))
                    diagnostics.Add(new MissmatchedTypeError(expr, leftType, binaryOperationResult));
            }

            return new BoundAssignmentExpression(typeSystem, left, operation, right);
        }

        private BoundExpression BindCallExpression(CallExpression expr)
        {
            var bound = BindExpression(expr.Expression);
            var args = BindArgumentExpression(expr.Arguments);

            if (bound.ResultType is FunctionGroupType functionGroup)
            {
                var function = FindFunctionOverload(functionGroup, args.Arguments.Select(arg => arg.ResultType));

                if (function is null)
                {
                    diagnostics.Add(new NoMatchingOverloadError(functionGroup.FullName(), expr, args));
                    return new BoundInvalidCallExpression(typeSystem, bound);
                }

                return new BoundFunctionCallExpression(function, bound, args);
            }
            else if (bound.ResultType is MethodGroupType methodGroup)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                if (bound.ResultType is not ErrorType)
                    diagnostics.Add(new NotCallableError(expr.Expression));
                return new BoundInvalidCallExpression(typeSystem, bound);
            }
        }

        private BoundArguments BindArgumentExpression(ArgumentExpression arguments)
        {
            var builder = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach (var arg in arguments.Nodes)
            {
                var bound = BindExpression(arg);
                builder.Add(bound);
            }

            return new BoundArguments(builder.ToImmutable());
        }

        private BoundExpression BindSubscriptExpression(SubscriptExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindMemberAccessExpression(MemberAccessExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindIdentifierExpression(IdentifierExpression expr)
        {
            var name = ExtractName(expr.NameToken);

            var symbol = LookupSymbol(name);

            if (symbol is not null)
                return new BoundSymbolExpression(symbol);

            if (symbol is not ErrorSymbol)
                diagnostics.Add(new NonExistantNameError(expr.NameToken));
            return new BoundInvalidExpression(typeSystem);
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

            diagnostics.Add(new UnsupportedBinaryOperation(expr, left.ResultType, right.ResultType));

            return new BoundBinaryExpression(left, op, right, typeSystem.Error);
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expr)
        {
            var childExpression = BindExpression(expr.Expression);
            var op = SemanticInfo.GetUnaryOperation(expr.OperatorToken.Type);

            var resultType = GetUnaryOperationResult(op, childExpression);

            if (resultType is not null)
                return new BoundUnaryExpression(childExpression, op, resultType);

            diagnostics.Add(new UnsupportedUnaryOperation(expr, childExpression.ResultType));

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

        private FunctionType? FindFunctionOverload(FunctionGroupType group, IEnumerable<TypeSpecifier> parameterTypes)
        {
            foreach (var func in group.Functions)
            {
                if (TypeSequenceEquals(func.Parameters.Select(param => param.Type), parameterTypes))
                    return func;
            }

            return null;
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