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
        private readonly Stack<SymbolScope> scopes;

        public LocalVariableScopeStack()
        {
            scopes = new Stack<SymbolScope>();
            scopes.Push(new SymbolScope());
        }

        public void EnterNewScope()
        {
            scopes.Push(new SymbolScope());
        }

        public void ExitCurrentScope()
        {
            scopes.Pop();
        }

        public bool TryDeclareLocal(string name, TypeSpecifier type, bool isConst)
        {
            return scopes.Peek().DeclareSymbol(new LocalVariableSymbol(name, type, isConst));
        }

        public LocalVariableSymbol? LookupLocal(string name)
        {
            foreach (var scope in scopes)
                if (scope.LookupSymbol(name) is LocalVariableSymbol symbol)
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

        private bool IsTypeMissmatch(TypeSpecifier expectedType, TypeSpecifier rightType)
        {
            if (expectedType is ErrorType || rightType is ErrorType)
                return false;

            return !expectedType.Equals(rightType);
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
            var (condition, _) = BindAndExpect(stmt.Condition, mustBeValue: true, mustBeModifiable: false, typeSystem.Bool);

            var body = BindStatement(stmt.Body);
            var elseClasue = stmt.ElseClause == null ? null : BindStatement(stmt.ElseClause.Body);

            return new BoundIfStatement(condition, body, elseClasue);
        }

        private BoundStatement BindDeclarationStatement(DeclarationStatement stmt)
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

            if (!locals.TryDeclareLocal(name, varType, isConst))
                diagnostics.Add(new RedefinedLocalVariableError(stmt.NameToken));

            var localVariable = locals.LookupLocal(name)!;

            return new BoundDeclarationStatement(localVariable, typeClause, expr);
        }

        private BoundTypeClause BindTypeClause(TypeClause typeClause)
        {
            var expr = BindExpression(typeClause.NameExpression);

            var exprNodeType = expr.Type;

            if (exprNodeType == BoundNodeType.BoundTypeNameExpression || exprNodeType == BoundNodeType.BoundTypeNameMemberAccessExpression)
                return new BoundTypeClause(expr, expr.ResultType);

            if (expr.ResultType is not ErrorType)
                diagnostics.Add(new ExpectedTypeNameError(typeClause.NameExpression));

            return new BoundTypeClause(expr, typeSystem.Error);
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
            throw new NotImplementedException();
        }

        private BoundExpression BindSubscriptExpression(SubscriptExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindMemberAccessExpression(MemberAccessExpression expr)
        {
            var right = BindExpression(expr.Expression);
            var name = ExtractName(expr.IdentifierExpression.NameToken);

            if (right.ResultType is ErrorType)
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);

            if (!(right.ResultSymbol is IMemberContainer memberContainer))
            {
                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultSymbol, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }

            var members = memberContainer.LookupMembers(name);

            if (members.Count() == 0)
            {
                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultSymbol, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }

            if (right.IsValue)
            {
                // allowed members:
                // - non-static methods
                // - non-static fields => TODO
                // - non-static properties => TODO

                var instanceMethods = members
                    .Where(member => member.Kind == SymbolKind.MethodSymbol)
                    .Cast<IMethodSymbol>()
                    .Where(method => !method.IsStatic);

                throw new System.NotImplementedException();
            }
            else
            {
                // allowed members:
                // - static methods
                // - static fields => TODO
                // - static properties => TODO
                // - type names
                // - namespaces

                var staticMethods = members
                    .Where(member => member.Kind == SymbolKind.MethodSymbol)
                    .Cast<IMethodSymbol>()
                    .Where(method => method.IsStatic);

                var staticFields = members
                    .Where(member => member.Kind == SymbolKind.FieldSymbol)
                    .Cast<IFieldSymbol>()
                    .Where(field => field.IsStatic);

                var staticProperties = members
                    .Where(member => member.Kind == SymbolKind.PropertySymbol)
                    .Cast<IPropertySymbol>()
                    .Where(property => property.IsStatic);

                var typeNames = members
                    .Where(member => member.Kind == SymbolKind.ClassTypeSymbol)
                    .Cast<ITypeSymbol>();

                var namespaces = members
                    .Where(member => member.Kind == SymbolKind.NamespaceSymbol)
                    .Cast<INamespaceSymbol>();

                int staticMethodCount = staticMethods.Count();
                int staticFieldCount = staticFields.Count();
                int staticPropertyCount = staticProperties.Count();
                int typeNameCount = typeNames.Count();
                int namespaceCount = namespaces.Count();

                if (staticMethodCount > 0)
                {
                    throw new System.NotImplementedException();
                }

                if (staticFieldCount > 0)
                {
                    Debug.Assert(staticFieldCount == 1);
                    return new BoundFieldAccessExpression(staticFields.First(), right, name);
                }

                if (staticPropertyCount > 0)
                {
                    Debug.Assert(staticPropertyCount == 1);
                    return new BoundPropertyAccessExpression(staticProperties.First(), right, name);
                }

                if (typeNameCount > 0)
                {
                    return new BoundTypeNameMemberAccessExpression(typeNames.First(), right, name);
                }

                if (namespaceCount > 0)
                {
                    return new BoundNamespaceMemberAccessExpression(typeSystem, namespaces.First(), right, name);
                }

                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultSymbol, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }
        }

        private BoundExpression BindIdentifierExpression(IdentifierExpression expr)
        {
            var name = ExtractName(expr.NameToken);

            if (locals.LookupLocal(name) is LocalVariableSymbol localVariableSymbol)
                return new BoundLocalVariableExpression(localVariableSymbol);

            if (BuiltinTypeFromName(name) is ClassType builtinType)
                return new BoundTypeNameExpression(new TypeNameSymbol(builtinType));

            if (LookupGlobalNamespace(name) is Symbol symbol)
            {
                if (symbol is TypeNameSymbol typeSymbol)
                    return new BoundTypeNameExpression(typeSymbol);
                else if (symbol is NamespaceSymbol namespaceSymbol)
                    return new BoundNamespaceExpression(typeSystem, namespaceSymbol);
            }

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

        private Symbol? LookupGlobalNamespace(string name)
        {
            return typeSystem.RootNamespace.Scope.LookupSymbol(name);
        }

        private ClassType? BuiltinTypeFromName(string name)
        {
            switch (name)
            {
                case "char": return typeSystem.Char;
                case "i8": return typeSystem.I8;
                case "i16": return typeSystem.I16;
                case "i32": return typeSystem.I32;
                case "i64": return typeSystem.I64;
                case "u8": return typeSystem.U8;
                case "u16": return typeSystem.U16;
                case "u32": return typeSystem.U32;
                case "u64": return typeSystem.U64;
                case "float": return typeSystem.Float;
                case "double": return typeSystem.Double;
                case "bool": return typeSystem.Bool;
                case "string": return typeSystem.String;

                default: return null;
            }
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

            var name = SemanticInfo.GetMethodNameFromOperation(op);

            var methodGroup = leftType
                .ReadOnlyScope?
                .LookupSymbol(name)
                .Where(member => member is IOperationSymbol)
                .Cast<IOperationSymbol>()
                .Where(operation => operation.Operation == op)
                .Select(operation => operation.UnderlyingMethod)
                .Where(
                    method => method.Parameters.Count() == 2 &&
                    method.Parameters.First().Type.Equals(leftType) &&
                    method.Parameters.Last().Type.Equals(rightType)
                );

            if (methods.Count() > 1)
                throw new Exception($"Found multiple methods for an operator: op={op}, left={leftType}, right={rightType}");

            var method = methods.FirstOrDefault();

            if (method is null)
                return null;

            return method.ReturnType;
        }

        private TypeSpecifier? GetUnaryOperationResult(BoundOperation op, BoundExpression expr)
        {
            Debug.Assert(op.IsUnaryOperation());

            var type = expr.ResultType;

            if (type is ErrorType)
                return typeSystem.Error;

            if (!expr.IsValue)
                return null;

            var name = SemanticInfo.GetMethodNameFromOperation(op);

            var methods = type
                .LookupMembers(name)
                .Where(member => member is IOperationSymbol)
                .Cast<IOperationSymbol>()
                .Where(operation => operation.Operation == op)
                .Select(operation => operation.UnderlyingMethod)
                .Where(
                    method => method.Parameters.Count() == 1 &&
                    method.Parameters.First().Type.Equals(type)
                );

            if (methods.Count() > 1)
                throw new Exception($"Found multiple methods for an operator: op={op}, type={type}");

            var method = methods.FirstOrDefault();

            if (method is null)
                return null;

            return method.ReturnType;
        }
    }
}