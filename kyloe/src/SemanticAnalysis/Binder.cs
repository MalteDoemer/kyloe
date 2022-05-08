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

        private readonly Stack<SyntaxToken> loopStack;

        public Binder(TypeSystem typeSystem, DiagnosticCollector diagnostics)
        {
            this.typeSystem = typeSystem;
            this.diagnostics = diagnostics;
            this.loopStack = new Stack<SyntaxToken>();
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

        private bool IsTypeMissmatch(TypeInfo expectedType, TypeInfo rightType)
        {
            if (expectedType is ErrorType || rightType is ErrorType)
                return false;

            return !expectedType.Equals(rightType);
        }

        private bool TypeSequenceEquals(IEnumerable<TypeInfo> seq1, IEnumerable<TypeInfo> seq2)
        {
            if (seq1.Count() != seq2.Count())
                return false;

            foreach (var (t1, t2) in seq1.Zip(seq2))
                if (!t1.Equals(t2))
                    return false;

            return true;
        }

        private CallableType? FindCallOverload(CallableGroupType group, IEnumerable<TypeInfo> parameterTypes)
        {
            foreach (var callable in group.Callables)
            {
                if (TypeSequenceEquals(callable.ParameterTypes, parameterTypes))
                    return callable;
            }

            return null;
        }

        private TypeInfo GetResultType(BoundExpression expr, SourceLocation src, TypeInfo? expectedType = null, bool mustBeValue = false, bool mustBeModifiableValue = false, bool mustBeTypeName = false)
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
                return new BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement>.Empty, ImmutableArray<BoundFunctionDefinition>.Empty, null, token);

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
                functionDefs.Add(BindFunctionDefinition(funcDecl));

            var mainSymbol = LookupSymbol("main");

            if (mainSymbol is CallableGroupSymbol mainGroup) 
            {
                // TODO: maybe give a warning here ?

                if (FindCallOverload(mainGroup.Group, Enumerable.Empty<TypeInfo>()) is FunctionType mainFunctionType) 
                {
                    var mainFunctionDef = functionDefs.Where(f => f.FunctionType.Equals(mainFunctionType)).FirstOrDefault();
                    return new BoundCompilationUnit(globals.ToImmutable(), functionDefs.ToImmutable(), mainFunctionDef, token);
                }
            }

            return new BoundCompilationUnit(globals.ToImmutable(), functionDefs.ToImmutable(), null, token);
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
            var groupSymbol = GetOrDeclareCallableGroup(nameTerminal);

            var functionType = new FunctionType(groupSymbol.Group, typeClause.Type);
            foreach (var param in parameters.Parameters)
                functionType.Parameters.Add(param.Symbol);

            bool sameOverloadExists = FindCallOverload(groupSymbol.Group, parameters.ParameterTypes) is not null;

            if (sameOverloadExists && parameters.AllParametersValid && !nameTerminal.Invalid)
                diagnostics.OverloadWithSameParametersExistsError(nameTerminal.Location, name);
            else
                groupSymbol.Group.Callables.Add(functionType);

            return new BoundFunctionDeclaration(functionType, parameters, typeClause, token);
        }

        private CallableGroupSymbol GetOrDeclareCallableGroup(SyntaxTerminal nameTerminal)
        {
            var name = nameTerminal.Text;
            var symbol = LookupSymbol(name);

            if (symbol is CallableGroupSymbol existingGroup)
                return existingGroup;

            if (symbol is not null && symbol is not ErrorSymbol && !nameTerminal.Invalid)
                diagnostics.NameAlreadyExistsError(nameTerminal.Location, name);

            var groupSymbol = new CallableGroupSymbol(new CallableGroupType(name, null));

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

            return new BoundParameters(builder.ToImmutable(), token);
        }

        private BoundParameterDeclaration BindParameterDeclaration(SyntaxToken token)
        {
            // ParameterDeclaration
            // ├── Identifier
            // └── TypeClause

            var param = GetNode(token, SyntaxTokenKind.ParameterDeclaration);
            var nameTerminal = GetTerminal(param.Tokens[0], SyntaxTokenKind.Identifier);
            var typeClause = BindTypeClause(GetNode(param.Tokens[1]));
            var symbol = new ParameterSymbol(nameTerminal.Text, typeClause.Type);

            return new BoundParameterDeclaration(symbol, token);
        }

        private BoundTypeClause BindFunctionTypeClause(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return new BoundTypeClause(typeSystem.Void, token);

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundTypeClause(typeSystem.Error, token);

            var typeClause = GetNode(token, SyntaxTokenKind.TrailingTypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return new BoundTypeClause(typeSystem.Error, token);
            }
            else if (symbol is not TypeNameSymbol)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.ExpectedTypeNameError(nameTerminal.Location);
                return new BoundTypeClause(typeSystem.Error, token);
            }

            return new BoundTypeClause(symbol.Type, token);
        }

        private BoundTypeClause BindTypeClause(SyntaxToken token)
        {
            // TypeClause
            // ├── Colon
            // └── Identifier

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundTypeClause(typeSystem.Error, token);

            var typeClause = GetNode(token, SyntaxTokenKind.TypeClause);
            var nameTerminal = GetTerminal(typeClause.Tokens[1], SyntaxTokenKind.Identifier);

            var symbol = LookupSymbol(nameTerminal.Text);

            if (symbol is null)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NonExistantNameError(nameTerminal.Location, nameTerminal.Text);
                return new BoundTypeClause(typeSystem.Error, token);
            }
            else if (symbol is not TypeNameSymbol)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.ExpectedTypeNameError(nameTerminal.Location);
                return new BoundTypeClause(typeSystem.Error, token);
            }

            return new BoundTypeClause(symbol.Type, token);
        }

        private BoundFunctionDefinition BindFunctionDefinition(BoundFunctionDeclaration decl)
        {
            // FunctionDefinition
            // ├── FuncKeyword
            // ├── Identifier
            // ├── LeftParen
            // ├── Parameters (optional)
            // ├── RightParen
            // ├── TypeClause (optional)
            // └── BlockStatement

            var function = GetNode(decl.Syntax, SyntaxTokenKind.FunctionDefinition);

            functionStack.Push(decl.FunctionType);
            EnterNewScope(); // this scope contains the parameters

            foreach (var paramDecl in decl.Parameters)
            {
                if (!DeclareSymbol(paramDecl.Symbol))
                {
                    diagnostics.RedefinedParameterError(paramDecl.Syntax.Location, paramDecl.Symbol.Name);
                }
            }

            var body = GetNode(function.Tokens[6]);
            var boundBody = BindBlockStatement(body);

            ExitCurrentScope();
            functionStack.Pop();

            return new BoundFunctionDefinition(decl, boundBody, decl.Syntax);
        }

        private BoundStatement BindStatement(SyntaxToken token)
        {
            switch (token.Kind)
            {
                case SyntaxTokenKind.Error:
                    return new BoundInvalidStatement(token);
                case SyntaxTokenKind.SemiColon:
                    return new BoundEmptyStatement(token);
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

        private BoundContinueStatement BindContinueStatement(SyntaxToken token)
        {
            if (loopStack.Count == 0)
                diagnostics.IllegalContinueStatement(token.Location);

            return new BoundContinueStatement(token);
        }

        private BoundBreakStatement BindBreakStatement(SyntaxToken token)
        {
            if (loopStack.Count == 0)
                diagnostics.IllegalBreakStatement(token.Location);

            return new BoundBreakStatement(token);
        }

        private BoundReturnStatement BindReturnStatement(SyntaxToken token)
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

                var expr = hasExpr ? BindExpression(exprSyntax) : null;
                return new BoundReturnStatement(expr, token);
            }

            var function = functionStack.Peek();

            if (hasExpr)
            {
                var expr = BindExpression(exprSyntax);
                var _result = GetResultType(expr, exprSyntax.Location, function.ReturnType, mustBeValue: true);
                return new BoundReturnStatement(expr, token);
            }
            else
            {
                if (IsTypeMissmatch(function.ReturnType, typeSystem.Void) && !returnKeyword.Invalid)
                    diagnostics.MissmatchedTypeError(token.Location, function.ReturnType, typeSystem.Void);

                return new BoundReturnStatement(null, token);
            }
        }

        private BoundIfStatement BindIfStatement(SyntaxToken token)
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

            return new BoundIfStatement(expr, body, elifStatement, token);
        }

        private BoundStatement BindElifStatement(SyntaxToken token)
        {
            if (token.Kind == SyntaxTokenKind.Epsilon)
                return new BoundEmptyStatement(token);
            else if (token.Kind == SyntaxTokenKind.Error)
                return new BoundInvalidStatement(token);

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

                return new BoundIfStatement(expr, body, elifStatement, token);
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

        private BoundWhileStatement BindWhileStatement(SyntaxToken token)
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

            loopStack.Push(whileStatement);
            var body = BindStatement(blockSyntax);
            loopStack.Pop();

            return new BoundWhileStatement(expr, body, token);
        }

        private BoundExpressionStatement BindExpressionStatement(SyntaxToken token)
        {
            // ExpressionStatement
            // ├── Expression
            // └── SemiColon

            var statement = GetNode(token, SyntaxTokenKind.ExpressionStatement);
            var bound = BindExpression(statement.Tokens[0]);
            return new BoundExpressionStatement(bound, token);
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

            return new BoundDeclarationStatement(symbol, expr, token);
        }

        private BoundBlockStatement BindBlockStatement(SyntaxToken token)
        {
            // BlockStatement
            // ├── LeftCurly
            // ├── RepeatedStatement
            // └── RightCurly

            if (token.Kind == SyntaxTokenKind.Error)
                return new BoundBlockStatement(ImmutableArray<BoundStatement>.Empty, token);

            var block = GetNode(token, SyntaxTokenKind.BlockStatement);

            var body = block.Tokens[1];

            var statements = CollectStatements(body);

            var builder = ImmutableArray.CreateBuilder<BoundStatement>();

            EnterNewScope();

            foreach (var stmt in statements)
                builder.Add(BindStatement(stmt));

            ExitCurrentScope();

            return new BoundBlockStatement(builder.ToImmutable(), token);
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
                    return new BoundInvalidExpression(typeSystem, token);
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

            var helper = GetNode(token, SyntaxTokenKind.AssignmentHelper);

            if (helper.Tokens[1].Kind != SyntaxTokenKind.Assignment)
                return BindExpression(helper.Tokens[0]);

            return BindAssignment(helper.Tokens[0], helper.Tokens[1]);
        }

        private BoundAssignmentExpression BindAssignment(SyntaxToken toAssign, SyntaxToken token)
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

            return new BoundAssignmentExpression(typeSystem, left, op, right, token);
        }

        private BoundBinaryExpression BindBinary(SyntaxToken token)
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

            if (resultType is null)
            {
                if (!opTerminal.Invalid)
                    diagnostics.UnsupportedBinaryOperation(token.Location, op, left.ResultType, right.ResultType);

                resultType = typeSystem.Error;
            }

            return new BoundBinaryExpression(left, op, right, resultType, token);
        }

        private BoundUnaryExpression BindPrefix(SyntaxToken token)
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


            if (resultType is null)
            {
                if (!opTerminal.Invalid)
                    diagnostics.UnsupportedUnaryOperation(token.Location, op, expr.ResultType);

                resultType = typeSystem.Error;
            }

            return new BoundUnaryExpression(expr, op, resultType, token);
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

        private BoundCallExpression BindCallExpression(SyntaxToken token)
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

            if (expr.ResultType is CallableGroupType callGroup)
            {
                var callable = FindCallOverload(callGroup, args.ArgumentTypes);

                if (callable is null)
                {
                    if (args.AllArgumentsValid)
                        diagnostics.NoMatchingOverloadError(exprSyntax.Location, callGroup.FullName(), args);
                    return new BoundCallExpression(typeSystem.Error, typeSystem.Error, expr, args, token);
                }

                return new BoundCallExpression(callable, callable.ReturnType, expr, args, token);
            }
            else
            {
                if (expr.ResultType is not ErrorType)
                    diagnostics.NotCallableError(exprSyntax.Location);
                return new BoundCallExpression(typeSystem.Error, typeSystem.Error, expr, args, token);
            }
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

            return new BoundArguments(builder.ToImmutable(), token);
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

        private BoundParenthesizedExpression BindParenthesized(SyntaxToken token)
        {
            // Parenthesized
            // ├── LeftParen
            // ├── Expression
            // └── RightParen

            var node = GetNode(token, SyntaxTokenKind.Parenthesized);
            var expr = BindExpression(node.Tokens[1]);

            return new BoundParenthesizedExpression(expr, token);
        }

        private BoundLiteralExpression BindLiteral(SyntaxToken token)
        {
            var terminal = GetTerminal(token);
            var type = SemanticInfo.GetTypeFromLiteral(typeSystem, token.Kind);
            var value = GetValueForLiteral(terminal, type);

            if (value is not null)
                return new BoundLiteralExpression(type, value, token);

            if (!terminal.Invalid)
                diagnostics.InvalidLiteralError(token.Location);

            return new BoundLiteralExpression(typeSystem.Error, new object(), token);
        }

        private object? GetValueForLiteral(SyntaxTerminal token, TypeInfo type)
        {
            var text = token.Text;

            switch (token.Kind)
            {
                case SyntaxTokenKind.Int:
                    if (long.TryParse(text, out var res1))
                        return res1;
                    return null;
                case SyntaxTokenKind.Float:
                    if (double.TryParse(text, out var res2))
                        return res2;
                    return null;
                case SyntaxTokenKind.Bool:
                    if (bool.TryParse(text, out var res3))
                        return res3;
                    return null;
                case SyntaxTokenKind.String:
                    return token.Text;
                default:
                    throw new System.Exception($"Unexpected literal type: {token.Kind}");
            }
        }

        private BoundSymbolExpression BindIdentifier(SyntaxToken token)
        {
            var nameTerminal = GetTerminal(token, SyntaxTokenKind.Identifier);
            var name = nameTerminal.Text;
            var symbol = LookupSymbol(name);


            if (symbol is null)
            {
                if (!nameTerminal.Invalid)
                    diagnostics.NonExistantNameError(nameTerminal.Location, name);

                symbol = new ErrorSymbol(typeSystem);
            }

            return new BoundSymbolExpression(symbol, token);
        }

        private TypeInfo? GetBinaryOperationResult(BoundExpression left, BoundOperation op, BoundExpression right)
        {
            Debug.Assert(op.IsBinaryOperation());

            var leftType = left.ResultType;
            var rightType = right.ResultType;

            if (leftType is ErrorType || rightType is ErrorType)
                return typeSystem.Error;

            if (!left.IsValue || !right.IsValue)
                return null;

            var name = SemanticInfo.GetFunctionNameFromOperation(op);

            var group = leftType.ReadOnlyScope?.LookupSymbol(name) as CallableGroupSymbol;

            if (group is null)
                return null;

            var args = new[] { leftType, rightType };

            var method = FindCallOverload(group.Group, args) as MethodType;

            if (method is null || !method.IsOperator)
                return null;

            return method.ReturnType;
        }

        private TypeInfo? GetUnaryOperationResult(BoundOperation op, BoundExpression expr)
        {
            Debug.Assert(op.IsUnaryOperation());

            var type = expr.ResultType;

            if (type is ErrorType)
                return typeSystem.Error;

            if (!expr.IsValue)
                return null;

            var name = SemanticInfo.GetFunctionNameFromOperation(op);

            var group = type.ReadOnlyScope?.LookupSymbol(name) as CallableGroupSymbol;

            if (group is null)
                return null;

            var args = new[] { type };

            var method = FindCallOverload(group.Group, args) as MethodType;

            if (method is null || !method.IsOperator)
                return null;

            return method.ReturnType;
        }
    }
}