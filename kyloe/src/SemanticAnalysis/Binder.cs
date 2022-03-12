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

        private (BoundExpression, ISymbol) BindAndExpect(SyntaxExpression expr, bool mustBeValue, bool mustBeLValue, ITypeSymbol? expectedType = null)
        {
            var bound = BindExpression(expr);

            if (bound.ResultSymbol is IErrorTypeSymbol)
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

            if (expectedType is not null && expectedType is not IErrorTypeSymbol && expectedType != bound.ResultSymbol)
            {
                diagnostics.Add(new MissmatchedTypeError(expr, expectedType, bound.ResultSymbol));
                return (bound, typeSystem.Error);
            }

            return (bound, bound.ResultSymbol);
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

            if (stmt.TypeClause is not null)
            {
                varType = BindTypeClause(stmt.TypeClause);

                if (varType is not IErrorTypeSymbol && varType != exprType)
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

            if (!locals.TryDeclareLocal(name, exprType, isConst))
                diagnostics.Add(new RedefinedLocalVariableError(stmt.NameToken));

            var localVariable = locals.LookupLocal(name)!;

            return new BoundDeclarationStatement(localVariable, expr);
        }

        private ITypeSymbol BindTypeClause(TypeClause typeClause)
        {
            // TODO: make BoundTypeClause class 

            var expr = BindExpression(typeClause.NameExpression);

            if (expr is BoundTypeNameExpression typeName)
                return typeName.TypeSymbol;
            else if (expr is BoundTypeNameMemberAccessExpression typeNameMemberAccess)
                return typeNameMemberAccess.TypeSymbol;

            if (expr.ResultSymbol is not IErrorTypeSymbol)
                diagnostics.Add(new ExpectedTypeNameError(typeClause.NameExpression));

            return typeSystem.Error;
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
            var (left, leftType) = BindAndExpect(expr.LeftNode, mustBeValue: true, mustBeLValue: true);
            var (right, rightType) = BindAndExpect(expr.RightNode, mustBeValue: true, mustBeLValue: false);

            var operation = SemanticInfo.GetAssignmentOperation(expr.OperatorToken.Type);

            if (leftType is IErrorTypeSymbol || rightType is IErrorTypeSymbol)
                return new BoundAssignmentExpression(typeSystem, left, operation, right);

            if (operation == AssignmentOperation.Assign)
            {
                if (leftType != rightType)
                    diagnostics.Add(new MissmatchedTypeError(expr, leftType, rightType));
            }
            else
            {
                var binaryOperation = SemanticInfo.GetBinaryOperationForAssignment(operation);
                var binaryOperationResult = GetBinaryOperationResult(left, binaryOperation, right);

                if (binaryOperationResult is null)
                    diagnostics.Add(new UnsupportedAssignmentOperation(expr, leftType, rightType));
                else if (binaryOperationResult != leftType)
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

            if (right.ResultSymbol is IErrorTypeSymbol)
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);

            if (!(right.ResultSymbol is ISymbolContainer symbolContainer))
            {
                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultSymbol, name));
                return new BoundInvalidMemberAccessExpression(typeSystem, right, name);
            }

            var members = symbolContainer.LookupMembers(name);

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
                    .Where(method => !method.IsStatic);

                var typeNames = members
                    .Where(member => member.Kind == SymbolKind.TypeSymbol)
                    .Cast<ITypeSymbol>();

                var namespaces = members
                    .Where(member => member.Kind == SymbolKind.NamespaceSymbol)
                    .Cast<INamespaceSymbol>();

                int staticMethodCount = staticMethods.Count();
                int typeNameCount = typeNames.Count();
                int namespaceCount = namespaces.Count();

                if (staticMethodCount > 0)
                {
                    Debug.Assert(typeNameCount == 0 && namespaceCount == 0);

                    throw new System.NotImplementedException();
                }

                if (typeNameCount > 0)
                {
                    Debug.Assert(namespaceCount == 0 && typeNameCount == 1);
                    return new BoundTypeNameMemberAccessExpression(typeNames.First(), right, name);
                }

                if (namespaceCount > 0)
                {
                    Debug.Assert(namespaceCount == 1);
                    return new BoundNamespaceMemberAccessExpression(namespaces.First(), right, name);
                }

                diagnostics.Add(new MemberNotFoundError(expr.Expression, right.ResultSymbol, name));
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
                    return new BoundNamespaceExpression(namespaceSymbol);
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