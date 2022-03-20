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

        public bool TryDeclareLocal(string name, ITypeSymbol type, bool isConst)
        {
            return scopes.Peek().TryDeclareLocal(name, type, isConst);
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

        private bool IsTypeMissmatch(ITypeSymbol? expectedType, ITypeSymbol? rightType)
        {
            if (expectedType is null)
                return false;

            if (expectedType is IErrorTypeSymbol || rightType is IErrorTypeSymbol)
                return false;

            return !expectedType.Equals(rightType);
        }

        private (BoundExpression, ITypeSymbol) BindAndExpect(SyntaxExpression expr, bool mustBeValue, bool mustBeLValue, ITypeSymbol? expectedType = null)
        {
            var bound = BindExpression(expr);

            if (bound.ResultType is IErrorTypeSymbol)
                return (bound, typeSystem.Error);

            if (mustBeValue && !bound.IsValue)
            {
                diagnostics.Add(new ExpectedValueError(expr));
                return (bound, typeSystem.Error);
            }

            if (mustBeLValue && !bound.IsLValue)
            {
                diagnostics.Add(new ExpectedLValueError(expr));
                return (bound, typeSystem.Error);
            }

            if (IsTypeMissmatch(expectedType, bound.ResultType))
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
            var (condition, _) = BindAndExpect(stmt.Condition, mustBeValue: true, mustBeLValue: false, typeSystem.Bool);

            var body = BindStatement(stmt.Body);
            var elseClasue = stmt.ElseClause == null ? null : BindStatement(stmt.ElseClause.Body);

            return new BoundIfStatement(condition, body, elseClasue);
        }

        private BoundStatement BindDeclarationStatement(DeclarationStatement stmt)
        {
            bool isConst = stmt.DeclerationToken.Type == SyntaxTokenType.ConstKeyword;

            var (expr, symbol) = BindAndExpect(stmt.AssignmentExpression, mustBeValue: true, mustBeLValue: false);
            var exprType = (ITypeSymbol)symbol;

            ITypeSymbol varType;
            BoundTypeClause? typeClause = null;

            if (stmt.TypeClause is not null)
            {
                typeClause = BindTypeClause(stmt.TypeClause);
                varType = typeClause.TypeSymbol;

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

            if (expr is BoundTypeNameExpression typeName)
                return new BoundTypeClause(expr, typeName.TypeSymbol);
            else if (expr is BoundTypeNameMemberAccessExpression typeNameMemberAccess)
                return new BoundTypeClause(expr, typeNameMemberAccess.TypeSymbol);

            if (expr.ResultType is not IErrorTypeSymbol)
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
            var (left, leftSymbol) = BindAndExpect(expr.LeftNode, mustBeValue: true, mustBeLValue: true);
            var (right, rightSymbol) = BindAndExpect(expr.RightNode, mustBeValue: true, mustBeLValue: false);

            var leftType = (ITypeSymbol)leftSymbol;
            var rightType = (ITypeSymbol)rightSymbol;

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

            if (right.ResultType is IErrorTypeSymbol)
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);

            IMemberContainer memberContainer;

            if (right is BoundNamespaceExpression nsExpression)
                memberContainer = nsExpression.NamespaceSymbol;
            else if (right is BoundNamespaceMemberAccessExpression namespaceMemberAccessExpression)
                memberContainer = namespaceMemberAccessExpression.NamespaceSymbol;
            else if (right.ResultType is IMemberContainer m)
                memberContainer = m;
            else
            {
                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultType, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }

            var members = memberContainer.LookupMembers(name);

            if (members.Count() == 0)
            {
                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultType, name));
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
                    return new BoundFieldMemberAccessExpression(staticFields.First(), right, name);
                }

                if (staticPropertyCount > 0)
                {
                    throw new System.NotImplementedException();
                }

                if (typeNameCount > 0)
                {
                    return new BoundTypeNameMemberAccessExpression(typeNames.First(), right, name);
                }

                if (namespaceCount > 0)
                {
                    return new BoundNamespaceMemberAccessExpression(typeSystem, namespaces.First(), right, name);
                }

                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultType, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }
        }

        private BoundExpression BindIdentifierExpression(IdentifierExpression expr)
        {
            var name = ExtractName(expr.NameToken);

            if (locals.LookupLocal(name) is ILocalVariableSymbol localVariableSymbol)
                return new BoundLocalVariableExpression(localVariableSymbol);

            if (BuiltinTypeFromName(name) is ITypeSymbol builtinType)
                return new BoundTypeNameExpression(builtinType);

            if (LookupGlobalNamespace(name) is ISymbol symbol)
            {
                if (symbol is ITypeSymbol typeSymbol)
                    return new BoundTypeNameExpression(typeSymbol);
                else if (symbol is INamespaceSymbol namespaceSymbol)
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

        private ISymbol? LookupGlobalNamespace(string name)
        {
            var members = typeSystem.RootNamespace.LookupMembers(name);
            Debug.Assert(members.Count() <= 1);
            return members.FirstOrDefault();
        }

        private ITypeSymbol? BuiltinTypeFromName(string name)
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

        private ITypeSymbol? GetBinaryOperationResult(BoundExpression left, BoundOperation op, BoundExpression right)
        {
            Debug.Assert(op.IsBinaryOperation());

            var leftType = left.ResultType;
            var rightType = right.ResultType;

            if (leftType is IErrorTypeSymbol || rightType is IErrorTypeSymbol)
                return typeSystem.Error;

            if (!left.IsValue || !right.IsValue)
                return null;


            // if (!(left.ResultSymbol is ITypeSymbol leftType) || !(right.ResultSymbol is ITypeSymbol rightType))
            //     return null;

            var name = SemanticInfo.GetMethodNameFromOperation(op);

            var methods = leftType
                .LookupMembers(name)
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

        private ITypeSymbol? GetUnaryOperationResult(BoundOperation op, BoundExpression expr)
        {
            Debug.Assert(op.IsUnaryOperation());

            var type = expr.ResultType;

            if (type is IErrorTypeSymbol)
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