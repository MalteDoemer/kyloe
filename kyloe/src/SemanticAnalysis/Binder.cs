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

        private readonly Stack<FunctionType> functionStack;

        public Binder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
        {
            this.typeSystem = typeSystem;
            this.diagnostics = diagnostics;
            this.functionStack = new Stack<FunctionType>();
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

        private FunctionType? FindFunctionOverload(FunctionGroupType group, IEnumerable<TypeSpecifier> parameterTypes)
        {
            foreach (var func in group.Functions)
            {
                if (TypeSequenceEquals(func.Parameters.Select(param => param.Type), parameterTypes))
                    return func;
            }

            return null;
        }

        private TypeSpecifier GetResultType(BoundExpression expr, SourceLocation src, TypeSpecifier? expectedType = null, bool mustBeValue = false, bool mustBeModifiableValue = false, bool mustBeTypeName = false)
        {
            if (expr.ResultType is ErrorType)
                return typeSystem.Error;

            if (expectedType is not null && IsTypeMissmatch(expectedType, expr.ResultType))
            {
                diagnostics.MissmatchedTypeError(src, expectedType, expr.ResultType);
                return typeSystem.Error;
            }

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

        public BoundCompilationUnit BindCompilationUnit(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return new BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement>.Empty, ImmutableArray<BoundFunctionDefinition>.Empty, null);

            var functionSyntax = Collect(token, SyntaxTokenKind.CompilationUnit, SyntaxTokenKind.FunctionDefinition).Where(t => t.Kind == SyntaxTokenKind.FunctionDefinition);
            var globalSyntax = Collect(token, SyntaxTokenKind.CompilationUnit, SyntaxTokenKind.DeclarationStatement).Where(t => t.Kind == SyntaxTokenKind.DeclarationStatement);

            var functionDecls = new List<BoundFunctionDeclaration>();
            var globals = ImmutableArray.CreateBuilder<BoundDeclarationStatement>();
            var functionDefs = ImmutableArray.CreateBuilder<BoundFunctionDefinition>();

            foreach (var func in functionSyntax)
                functionDecls.Add(BindFunctionDeclaration(func));

            foreach (var global in globalSyntax)
                globals.Add(BindDeclarationStatement(global));

            foreach (var (funcDecl, funcSyntax) in functionDecls.Zip(functionSyntax))
                functionDefs.Add(BindFunctionDefinition(funcSyntax, funcDecl));

            if (LookupSymbol("main") is FunctionGroupSymbol mainSymbol && FindFunctionOverload(mainSymbol.Group, Enumerable.Empty<TypeSpecifier>()) is FunctionType mainType)
            {
                var mainFunction = functionDefs.Where(f => f.Type.Equals(mainType)).FirstOrDefault();
                return new BoundCompilationUnit(globals.ToImmutable(), functionDefs.ToImmutable(), mainFunction);
            }

            return new BoundCompilationUnit(globals.ToImmutable(), functionDefs.ToImmutable(), null);
        }

        private BoundFunctionDeclaration BindFunctionDeclaration(SyntaxToken token)
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
            var parameterSyntax = GetNode(functionDeclaration.Tokens[3]);
            var typeClauseSyntax = GetNode(functionDeclaration.Tokens[5]);


            var typeClause = BindFunctionTypeClause(typeClauseSyntax);

            var parameters = BindParameters(parameterSyntax);

            var name = nameTerminal.Text;
            var groupSymbol = GetOrDeclareFunctionGroup(nameTerminal);

            var functionType = new FunctionType(name, groupSymbol.Group, typeClause.Type);
            foreach (var param in parameters.Parameters)
                functionType.Parameters.Add(param.Symbol);

            bool sameOverloadExists = FindFunctionOverload(groupSymbol.Group, parameters.ParameterTypes) is not null;

            if (sameOverloadExists && parameters.AllParametersValid && !nameTerminal.Invalid)
                diagnostics.OverloadWithSameParametersExistsError(nameTerminal.Location, name);
            else
                groupSymbol.Group.Functions.Add(functionType);

            return new BoundFunctionDeclaration(functionType, parameters, typeClause);
        }

        private FunctionGroupSymbol GetOrDeclareFunctionGroup(SyntaxTerminal nameTerminal)
        {
            var name = nameTerminal.Text;
            var symbol = LookupSymbol(name);

            if (symbol is FunctionGroupSymbol existingGroup)
                return existingGroup;

            if (symbol is not null && symbol is not ErrorSymbol && !nameTerminal.Invalid)
                diagnostics.NameAlreadyExistsError(nameTerminal.Location, name);

            var groupSymbol = new FunctionGroupSymbol(new FunctionGroupType(name));

            DeclareSymbol(groupSymbol); // Note: declare symbol does nothing if it already exists

            return groupSymbol;
        }

        private BoundParameters BindParameters(SyntaxToken token)
        {
            var parameters = Collect(token, SyntaxTokenKind.Parameters, SyntaxTokenKind.ParameterDeclaration)
                             .Where(t => t.Kind == SyntaxTokenKind.ParameterDeclaration);

            var builder = ImmutableArray.CreateBuilder<BoundParameterDeclaration>();

            foreach (var param in parameters)
                builder.Add(BindParameterDeclaration(param));

            return new BoundParameters(builder.ToImmutable());
        }

        private BoundParameterDeclaration BindParameterDeclaration(SyntaxToken token)
        {
            // ParameterDeclaration
            // ├── Identifier
            // └── TypeClause

            var param = GetNode(token, SyntaxTokenKind.ParameterDeclaration);
            var nameTerminal = GetTerminal(param.Tokens[0], SyntaxTokenKind.Identifier);
            var typeClause = BindTypeClause(GetNode(param.Tokens[1]));
            var symbol = new ParameterSymbol(nameTerminal.Text, typeClause.Type, nameTerminal.Location);

            return new BoundParameterDeclaration(symbol);
        }

        private BoundTypeClause BindFunctionTypeClause(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return new BoundTypeClause(typeSystem.Void);

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundTypeClause(typeSystem.Error);

            var typeClause = GetNode(token, SyntaxTokenKind.TrailingTypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return new BoundTypeClause(typeSystem.Error);
            }
            else if (symbol is not TypeNameSymbol)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.ExpectedTypeNameError(nameTerminal.Location);
                return new BoundTypeClause(typeSystem.Error);
            }

            return new BoundTypeClause(symbol.Type);
        }

        private BoundTypeClause BindTypeClause(SyntaxToken token)
        {
            // TypeClause
            // ├── Colon
            // └── Identifier

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundTypeClause(typeSystem.Error);

            var typeClause = GetNode(token, SyntaxTokenKind.TypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return new BoundTypeClause(typeSystem.Error);
            }
            else if (symbol is not TypeNameSymbol)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.ExpectedTypeNameError(nameTerminal.Location);
                return new BoundTypeClause(typeSystem.Error);
            }

            return new BoundTypeClause(symbol.Type);
        }

        private BoundFunctionDefinition BindFunctionDefinition(SyntaxToken token, BoundFunctionDeclaration decl)
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

            functionStack.Push(decl.Type);
            EnterNewScope(); // this scope contains the parameters

            foreach (var param in decl.Type.Parameters)
            {
                if (!DeclareSymbol(param))
                    if (param.Location is SourceLocation loc)
                        diagnostics.RedefinedParameterError(loc, param.Name);
            }

            var body = GetNode(function.Tokens[6]);
            var boundBody = BindBlockStatement(body);

            ExitCurrentScope();
            functionStack.Pop();

            return new BoundFunctionDefinition(decl, boundBody);
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
                case SyntaxTokenKind.WhileStatement:
                    return BindWhileStatement(token);
                case SyntaxTokenKind.ReturnStatement:
                    return BindReturnStatement(token);
                case SyntaxTokenKind.BreakStatement:
                    return BindBreakStatement(token);
                case SyntaxTokenKind.ContinueStatement:
                    return BindContinueStatement(token);
                default:
                    throw new Exception($"unexpected kind: {token.Kind}");
            }
        }

        private BoundStatement BindContinueStatement(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundStatement BindBreakStatement(SyntaxToken token)
        {
            throw new NotImplementedException();
        }

        private BoundStatement BindReturnStatement(SyntaxToken token)
        {
            // ReturnStatement
            // ├── ReturnKeyword
            // ├── Expression (optional)
            // └── SemiColon

            var returnStatement = GetNode(token, SyntaxTokenKind.ReturnStatement);
            var returnKeyword = GetTerminal(returnStatement.Tokens[0], SyntaxTokenKind.ReturnKeyword);
            var exprSyntax = returnStatement.Tokens[1];
            bool hasExpr = exprSyntax.Kind != SyntaxTokenKind.Epsilon;


            if (functionStack.Count == 0)
            {
                if (!returnKeyword.Invalid)
                    diagnostics.IllegalReturnStatement(token.Location);

                return new BoundInvalidStatement();
            }

            var function = functionStack.Peek();


            if (hasExpr)
            {
                var expr = BindExpression(exprSyntax);
                var _result = GetResultType(expr, exprSyntax.Location, function.ReturnType, mustBeValue: true);
                return new BoundReturnStatement(expr);
            }
            else
            {
                if (IsTypeMissmatch(function.ReturnType, typeSystem.Void))
                {
                    if (!returnKeyword.Invalid)
                        diagnostics.MissmatchedTypeError(token.Location, function.ReturnType, typeSystem.Void);

                    return new BoundInvalidStatement();
                }

                return new BoundReturnStatement(null);
            }
        }

        private BoundStatement BindIfStatement(SyntaxToken token)
        {
            // IfStatement
            // ├── IfKeyword
            // ├── Expr
            // ├── BlockStatement
            // └── ElifStatement (optional)

            var ifStatement = GetNode(token, SyntaxTokenKind.IfStatement);

            var exprSyntax = ifStatement.Tokens[1];
            var blockSyntax = ifStatement.Tokens[2];
            var elifSyntax = ifStatement.Tokens[3];

            var expr = BindExpression(exprSyntax);
            var _result = GetResultType(expr, exprSyntax.Location, typeSystem.Bool, mustBeValue: true);

            var body = BindStatement(blockSyntax);
            var elifStatement = BindElifStatement(elifSyntax);

            return new BoundIfStatement(expr, body, elifStatement);
        }

        private BoundStatement BindElifStatement(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return new BoundEmptyStatement();
            else if (token.Kind == SyntaxTokenKind.Error)
                return new BoundInvalidStatement();

            // ElifStatement
            // ├── ElifClause / ElseKeyword
            // ├── BlockStatement
            // │   ├── LeftCurly
            // │   ├── Epsilon
            // │   └── RightCurly
            // └── ElifStatement (Optional)

            var statement = GetNode(token, SyntaxTokenKind.ElifStatement);
            var blockSyntax = statement.Tokens[1];
            var elifSyntax = statement.Tokens[2];

            bool isElif = statement.Tokens[0].Kind == SyntaxTokenKind.ElifClause;

            if (isElif)
            {
                var clause = GetNode(statement.Tokens[0], SyntaxTokenKind.ElifClause);
                var exprSyntax = clause.Tokens[1];

                var expr = BindExpression(exprSyntax);
                var _result = GetResultType(expr, exprSyntax.Location, typeSystem.Bool, mustBeValue: true);

                var body = BindStatement(blockSyntax);
                var elifStatement = BindElifStatement(elifSyntax);

                return new BoundIfStatement(expr, body, elifStatement);
            }
            else
            {
                // HACK:
                // There is a little problem in the parser, the statement:
                //
                //      if true {} else { } elif false { } else { }
                // 
                // would be correct, i.e. you can have as many else statements as you want.
                // This is because the parser generator cannot deal with indirect recursion
                // and the grammar had to be changed accordingly.
                //
                // This error is checked here in the Binder.

                bool isIllegalSyntax = elifSyntax.Kind == SyntaxTokenKind.ElifStatement;

                if (isIllegalSyntax)
                {
                    bool isElse = GetNode(elifSyntax).Tokens[0].Kind == SyntaxTokenKind.ElseKeyword;

                    if (isElse)
                        diagnostics.IllegalElseStatement(elifSyntax.Location);
                    else
                        diagnostics.IllegalElifStatement(elifSyntax.Location);

                    var body = BindStatement(blockSyntax);

                    // still bind the elif statment to catch non-related errors.
                    BindElifStatement(elifSyntax);

                    return body;
                }

                return BindStatement(blockSyntax);
            }
        }

        private BoundStatement BindWhileStatement(SyntaxToken token)
        {
            // WhileStatement
            // ├── WhileKeyword
            // ├── Expr
            // └── BlockStatement

            var whileStatement = GetNode(token, SyntaxTokenKind.WhileStatement);

            var exprSyntax = whileStatement.Tokens[1];
            var blockSyntax = whileStatement.Tokens[2];

            var expr = BindExpression(exprSyntax);
            var _result = GetResultType(expr, exprSyntax.Location, typeSystem.Bool, mustBeValue: true);

            var body = BindStatement(blockSyntax);

            return new BoundWhileStatement(expr, body);
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
            // ├── TypeClause (optional)
            // ├── Equal
            // ├── Expr
            // └── SemiColon

            var declaration = GetNode(token, SyntaxTokenKind.DeclarationStatement);
            var declKeyword = GetTerminal(declaration.Tokens[0]);
            var nameTerminal = GetTerminal(declaration.Tokens[1], SyntaxTokenKind.Identifier);
            var typeClause = GetNode(declaration.Tokens[2]);
            var exprSyntax = declaration.Tokens[4];

            bool isConst = declKeyword.Kind == SyntaxTokenKind.ConstKeyword;
            bool hasTypeClause = typeClause.Kind != SyntaxTokenKind.Epsilon;

            var expr = BindExpression(exprSyntax);

            var expectedType = hasTypeClause ? BindTypeClause(typeClause).Type : null;

            var exprType = GetResultType(expr, exprSyntax.Location, expectedType, mustBeValue: true);

            var name = nameTerminal.Text;

            Symbol symbol;

            if (InGlobalScope())
                symbol = new GlobalVariableSymbol(name, exprType, isConst);
            else
                symbol = new LocalVariableSymbol(name, exprType, isConst);

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

            EnterNewScope();

            foreach (var stmt in statements)
                builder.Add(BindStatement(stmt));

            ExitCurrentScope();

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


            var leftType = GetResultType(left, toAssign.Location, mustBeValue: true, mustBeModifiableValue: true);
            var rightType = GetResultType(right, rightLocation, mustBeValue: true);

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

            var call = GetNode(token, SyntaxTokenKind.Postfix);
            var exprSyntax = call.Tokens[0];

            var args = BindArguments(call.Tokens[2]);
            var expr = BindExpression(exprSyntax);

            if (expr.ResultType is FunctionGroupType functionGroup)
            {
                var function = FindFunctionOverload(functionGroup, args.ArgumentTypes);

                if (function is null)
                {
                    if (args.AllArgumentsValid)
                        diagnostics.NoMatchingOverloadError(exprSyntax.Location, functionGroup.FullName(), args);
                    return new BoundInvalidExpression(typeSystem);
                }

                return new BoundFunctionCallExpression(function, expr, args);
            }
            else if (expr.ResultType is MethodGroupType methodGroup)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (expr.ResultType is not ErrorType)
                    diagnostics.NotCallableError(exprSyntax.Location);
                return new BoundInvalidExpression(typeSystem);
            }

            throw new NotImplementedException();
        }

        private IEnumerable<SyntaxToken> CollectArgs(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Error || token.Kind == SyntaxTokenKind.Epsilon)
                yield break;

            if (token.Kind == SyntaxTokenKind.Arguments)
            {
                // Arguments
                // ├── Expr
                // ├── Comma
                // └── Expr

                var args = GetNode(token, SyntaxTokenKind.Arguments);

                foreach (var child in CollectArgs(args.Tokens[0]))
                    yield return child;

                foreach (var child in CollectArgs(args.Tokens[2]))
                    yield return child;
            }
            else
            {
                yield return token;
            }
        }

        private BoundArguments BindArguments(SyntaxToken token)
        {
            var builder = ImmutableArray.CreateBuilder<BoundExpression>();
            var args = CollectArgs(token);

            foreach (var arg in args)
            {
                var bound = BindExpression(arg);
                builder.Add(bound);
            }

            return new BoundArguments(builder.ToImmutable());
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