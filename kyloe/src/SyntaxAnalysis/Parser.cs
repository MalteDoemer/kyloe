using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Diagnostics;
using System;

namespace Kyloe.Syntax
{
    internal class Parser
    {
        private Lexer lexer;

        private DiagnosticCollector diagnostics;

        private bool isCurrentValid;
        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(Lexer lexer, DiagnosticCollector diagnostics)
        {
            this.diagnostics = diagnostics;
            this.lexer = lexer;
            current = lexer.NextToken();
            next = lexer.NextToken();
            isCurrentValid = current.Kind != SyntaxTokenKind.Invalid;
        }

        /// Returns the current Token and then advances to the next one.
        private SyntaxToken Advance()
        {
            var temp = current;
            current = next;
            next = lexer.NextToken();
            isCurrentValid = current.Kind != SyntaxTokenKind.Invalid;
            return temp;
        }

        private SyntaxToken Expect(SyntaxTokenKind kind)
        {
            if (current.Kind == kind)
                return Advance();

            // Don't report a new diagnostic if the lexer already did.
            if (isCurrentValid)
            {
                diagnostics.Add(new ExpectedTokenError(kind, current));
                isCurrentValid = false;
            }

            return new SyntaxToken(kind, current.Location, SyntaxInfo.GetDefaultValue(kind));
        }

        public SyntaxNode Parse()
        {
            return ParseCompilationUnit();
        }

        public SyntaxStatement ParseStatement()
        {
            var stmt = ParseStatementImpl();
            Expect(SyntaxTokenKind.End);
            return stmt;
        }

        public SyntaxExpression ParseExpression()
        {
            var expr = ParseExpressionImpl();
            Expect(SyntaxTokenKind.End);
            return expr;
        }

        private CompilationUnitSyntax ParseCompilationUnit() 
        {
            var functions = ImmutableArray.CreateBuilder<FunctionDefinition>();
            var globals = ImmutableArray.CreateBuilder<DeclarationStatement>();

            while (current.Kind != SyntaxTokenKind.End) 
            {
                if (current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.ConstKeyword)
                    globals.Add(ParseDeclarationStatement());
                else
                    functions.Add(ParseFunctionDefinition());
            }

            return new CompilationUnitSyntax(globals.ToImmutable(), functions.ToImmutable());
        }
        
        private FunctionDefinition ParseFunctionDefinition()
        {
            var funcToken = Expect(SyntaxTokenKind.FuncKeyword);
            var nameToken = Expect(SyntaxTokenKind.Identifier);
            var leftParen = Expect(SyntaxTokenKind.LeftParen);

            ParameterList parameters;

            if (current.Kind != SyntaxTokenKind.RightParen)
                parameters = ParseParameters();
            else
                parameters = ParameterList.Empty;

            var rightParen = Expect(SyntaxTokenKind.RightParen);

            TypeClause? typeClause = null;

            if (current.Kind == SyntaxTokenKind.SmallArrow)
                typeClause = ParseTypeClause(SyntaxTokenKind.SmallArrow);

            var body = ParseBlockStatement();

            return new FunctionDefinition(funcToken, nameToken, leftParen, parameters, rightParen, typeClause, body);
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            var nameToken = Expect(SyntaxTokenKind.Identifier);
            var typeClause = ParseTypeClause();

            return new ParameterDeclaration(nameToken, typeClause);
        }

        private TypeClause ParseTypeClause(SyntaxTokenKind kind = SyntaxTokenKind.Colon)
        {
            var seperatorToken = Expect(kind);
            var node = ParseNameExpression();

            return new TypeClause(seperatorToken, node);
        }

        private ParameterList ParseParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterDeclaration>();
            var commas = ImmutableArray.CreateBuilder<SyntaxToken>();

            while (true)
            {
                parameters.Add(ParseParameterDeclaration());

                if (current.Kind != SyntaxTokenKind.Comma)
                    break;

                commas.Add(Advance());
            }

            return new ParameterList(parameters.ToImmutable(), commas.ToImmutable());
        }

        private ArgumentExpression ParseArguments()
        {
            var expressions = ImmutableArray.CreateBuilder<SyntaxExpression>();
            var commas = ImmutableArray.CreateBuilder<SyntaxToken>();

            while (true)
            {
                expressions.Add(ParseExpressionImpl());

                if (current.Kind != SyntaxTokenKind.Comma)
                    break;

                commas.Add(Advance());
            }

            return new ArgumentExpression(expressions.ToImmutable(), commas.ToImmutable());
        }

        private SyntaxExpression ParseNameExpression()
        {
            var identifierToken = Expect(SyntaxTokenKind.Identifier);

            SyntaxExpression node = new IdentifierExpression(identifierToken);

            while (current.Kind == SyntaxTokenKind.Dot)
            {
                var dotToken = Advance();
                var nameToken = Expect(SyntaxTokenKind.Identifier);
                node = new MemberAccessExpression(node, dotToken, new IdentifierExpression(nameToken));
            }

            return node;
        }

        private SyntaxStatement ParseStatementImpl()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.SemiColon:
                    return new EmptyStatement(Advance());
                case SyntaxTokenKind.LeftCurly:
                    return ParseBlockStatement();
                case SyntaxTokenKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxTokenKind.VarKeyword:
                case SyntaxTokenKind.ConstKeyword:
                    return ParseDeclarationStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private BlockStatement ParseBlockStatement()
        {
            var leftCurly = Expect(SyntaxTokenKind.LeftCurly);
            var builder = ImmutableArray.CreateBuilder<SyntaxStatement>();

            while (!(current.Kind == SyntaxTokenKind.RightCurly || current.Kind == SyntaxTokenKind.End))
            {
                var startToken = current;
                var stmt = ParseStatementImpl();
                builder.Add(stmt);

                // It could be that no token is consumed because 
                // of a MalformedExpression in ParsePrimary().
                // If this was the case we have to skip the token.
                if (object.ReferenceEquals(startToken, current))
                    Advance();
            }

            var rightCurly = Expect(SyntaxTokenKind.RightCurly);

            return new BlockStatement(leftCurly, builder.ToImmutable(), rightCurly);
        }

        private SyntaxStatement ParseIfStatement()
        {
            var ifToken = Expect(SyntaxTokenKind.IfKeyword);
            var condition = ParseExpressionImpl();
            var body = ParseBlockStatement();

            if (current.Kind == SyntaxTokenKind.ElseKeyword)
            {
                var elseToken = Advance();
                var elseBody = ParseBlockStatement();
                return new IfStatement(ifToken, condition, body, new ElseClause(elseToken, elseBody));
            }

            return new IfStatement(ifToken, condition, body, null);
        }

        private DeclarationStatement ParseDeclarationStatement()
        {
            SyntaxToken decl;

            if (current.Kind == SyntaxTokenKind.ConstKeyword)
                decl = Advance();
            else
                decl = Expect(SyntaxTokenKind.VarKeyword);

            var name = Expect(SyntaxTokenKind.Identifier);

            TypeClause? typeClause = null;

            if (current.Kind == SyntaxTokenKind.Colon)
                typeClause = ParseTypeClause();

            var equals = Expect(SyntaxTokenKind.Equals);
            var expr = ParseExpressionImpl();
            var semi = Expect(SyntaxTokenKind.SemiColon);
            return new DeclarationStatement(decl, name, typeClause, equals, expr, semi);
        }

        private SyntaxStatement ParseExpressionStatement()
        {
            var expr = ParseExpressionImpl();
            var semi = Expect(SyntaxTokenKind.SemiColon);
            return new ExpressionStatement(expr, semi);
        }

        private SyntaxExpression ParseExpressionImpl()
        {
            return ParseAssignmentExpression();
        }

        private SyntaxExpression ParseAssignmentExpression()
        {
            var left = ParseBinaryExpression();

            if (current.Kind.IsAssignmentOperator())
            {
                var op = Advance();
                var right = ParseAssignmentExpression();
                return new AssignmentExpression(left, op, right);
            }

            return left;
        }

        private SyntaxExpression ParseBinaryExpression(int precedence = SyntaxInfo.MAX_PRECEDENCE)
        {
            if (precedence == 0)
                return ParsePrefixExpression();

            var left = ParseBinaryExpression(precedence - 1);

            while (current.Kind.BinaryOperatorPrecedence() == precedence)
            {
                var op = Advance();
                var right = ParseBinaryExpression(precedence - 1);
                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        private SyntaxExpression ParsePrefixExpression()
        {
            if (current.Kind.IsPrefixOperator())
            {
                var op = Advance();
                var expr = ParsePrefixExpression();
                return new UnaryExpression(op, expr);
            }

            return ParsePostFixExpression();
        }

        private SyntaxExpression ParsePostFixExpression()
        {
            var node = ParsePrimary();

            while (current.Kind.IsPostfixOperator())
            {
                if (current.Kind == SyntaxTokenKind.LeftParen)
                {
                    var lparen = Expect(SyntaxTokenKind.LeftParen);

                    var arguments = ArgumentExpression.Empty;

                    if (current.Kind != SyntaxTokenKind.RightParen)
                        arguments = ParseArguments();

                    var rparen = Expect(SyntaxTokenKind.RightParen);
                    node = new CallExpression(node, lparen, arguments, rparen);
                }
                else if (current.Kind == SyntaxTokenKind.LeftSquare)
                {
                    var lsquare = Expect(SyntaxTokenKind.LeftSquare);
                    var expr = ParseExpressionImpl();
                    var rsqare = Expect(SyntaxTokenKind.RightSquare);

                    node = new SubscriptExpression(node, lsquare, expr, rsqare);
                }
                else if (current.Kind == SyntaxTokenKind.Dot)
                {
                    var dotToken = Advance();
                    var nameToken = Expect(SyntaxTokenKind.Identifier);
                    node = new MemberAccessExpression(node, dotToken, new IdentifierExpression(nameToken));
                }
            }

            return node;
        }

        private SyntaxExpression ParsePrimary()
        {
            if (current.Kind.IsLiteralToken())
            {
                return new LiteralExpression(Advance());
            }
            else if (current.Kind == SyntaxTokenKind.LeftParen)
            {
                var leftParen = Expect(SyntaxTokenKind.LeftParen);
                var expr = ParseExpressionImpl();
                var rightParen = Expect(SyntaxTokenKind.RightParen);

                return new ParenthesizedExpression(leftParen, expr, rightParen);
            }
            else if (current.Kind == SyntaxTokenKind.Identifier)
            {
                var name = Advance();
                return new IdentifierExpression(name);
            }
            else
            {
                // Don't report a new diagnostic if the lexer already did.
                if (isCurrentValid)
                {
                    diagnostics.Add(new ExpectedExpressionError(current));
                    isCurrentValid = false;
                }

                return new MalformedExpression(current);
            }
        }
    }
}