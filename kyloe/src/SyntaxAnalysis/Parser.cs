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
            isCurrentValid = current.Type != SyntaxTokenType.Invalid;
        }

        /// Returns the current Token and then advances to the next one.
        private SyntaxToken Advance()
        {
            var temp = current;
            current = next;
            next = lexer.NextToken();
            isCurrentValid = current.Type != SyntaxTokenType.Invalid;
            return temp;
        }

        private SyntaxToken Expect(SyntaxTokenType type)
        {
            if (current.Type == type)
                return Advance();

            // Don't report a new diagnostic if the lexer already did.
            if (isCurrentValid)
            {
                diagnostics.Add(new ExpectedTokenError(type, current));
                isCurrentValid = false;
            }

            return new SyntaxToken(type, current.Location, SyntaxInfo.GetDefaultValue(type));
        }

        public SyntaxNode Parse()
        {
            return ParseCompilationUnit();
        }

        public SyntaxStatement ParseStatement()
        {
            var stmt = ParseStatementImpl();
            Expect(SyntaxTokenType.End);
            return stmt;
        }

        public SyntaxExpression ParseExpression()
        {
            var expr = ParseExpressionImpl();
            Expect(SyntaxTokenType.End);
            return expr;
        }

        private CompilationUnitSyntax ParseCompilationUnit() 
        {
            var functions = ImmutableArray.CreateBuilder<FunctionDefinition>();
            var globals = ImmutableArray.CreateBuilder<DeclarationStatement>();

            while (current.Type != SyntaxTokenType.End) 
            {
                if (current.Type == SyntaxTokenType.VarKeyword || current.Type == SyntaxTokenType.ConstKeyword)
                    globals.Add(ParseDeclarationStatement());
                else
                    functions.Add(ParseFunctionDefinition());
            }

            return new CompilationUnitSyntax(globals.ToImmutable(), functions.ToImmutable());
        }
        
        private FunctionDefinition ParseFunctionDefinition()
        {
            var funcToken = Expect(SyntaxTokenType.FuncKeyword);
            var nameToken = Expect(SyntaxTokenType.Identifier);
            var leftParen = Expect(SyntaxTokenType.LeftParen);

            ParameterList parameters;

            if (current.Type != SyntaxTokenType.RightParen)
                parameters = ParseParameters();
            else
                parameters = ParameterList.Empty;

            var rightParen = Expect(SyntaxTokenType.RightParen);

            TypeClause? typeClause = null;

            if (current.Type == SyntaxTokenType.SmallArrow)
                typeClause = ParseTypeClause(SyntaxTokenType.SmallArrow);

            var body = ParseBlockStatement();

            return new FunctionDefinition(funcToken, nameToken, leftParen, parameters, rightParen, typeClause, body);
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            var nameToken = Expect(SyntaxTokenType.Identifier);
            var typeClause = ParseTypeClause();

            return new ParameterDeclaration(nameToken, typeClause);
        }

        private TypeClause ParseTypeClause(SyntaxTokenType type = SyntaxTokenType.Colon)
        {
            var colonToken = Expect(type);
            var node = ParseNameExpression();

            return new TypeClause(colonToken, node);
        }

        private ParameterList ParseParameters()
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterDeclaration>();
            var commas = ImmutableArray.CreateBuilder<SyntaxToken>();

            while (true)
            {
                parameters.Add(ParseParameterDeclaration());

                if (current.Type != SyntaxTokenType.Comma)
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

                if (current.Type != SyntaxTokenType.Comma)
                    break;

                commas.Add(Advance());
            }

            return new ArgumentExpression(expressions.ToImmutable(), commas.ToImmutable());
        }

        private SyntaxExpression ParseNameExpression()
        {
            var identifierToken = Expect(SyntaxTokenType.Identifier);

            SyntaxExpression node = new IdentifierExpression(identifierToken);

            while (current.Type == SyntaxTokenType.Dot)
            {
                var dotToken = Advance();
                var nameToken = Expect(SyntaxTokenType.Identifier);
                node = new MemberAccessExpression(node, dotToken, new IdentifierExpression(nameToken));
            }

            return node;
        }

        private SyntaxStatement ParseStatementImpl()
        {
            switch (current.Type)
            {
                case SyntaxTokenType.SemiColon:
                    return new EmptyStatement(Advance());
                case SyntaxTokenType.LeftCurly:
                    return ParseBlockStatement();
                case SyntaxTokenType.IfKeyword:
                    return ParseIfStatement();
                case SyntaxTokenType.VarKeyword:
                case SyntaxTokenType.ConstKeyword:
                    return ParseDeclarationStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private BlockStatement ParseBlockStatement()
        {
            var leftCurly = Expect(SyntaxTokenType.LeftCurly);
            var builder = ImmutableArray.CreateBuilder<SyntaxStatement>();

            while (!(current.Type == SyntaxTokenType.RightCurly || current.Type == SyntaxTokenType.End))
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

            var rightCurly = Expect(SyntaxTokenType.RightCurly);

            return new BlockStatement(leftCurly, builder.ToImmutable(), rightCurly);
        }

        private SyntaxStatement ParseIfStatement()
        {
            var ifToken = Expect(SyntaxTokenType.IfKeyword);
            var condition = ParseExpressionImpl();
            var body = ParseBlockStatement();

            if (current.Type == SyntaxTokenType.ElseKeyword)
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

            if (current.Type == SyntaxTokenType.ConstKeyword)
                decl = Advance();
            else
                decl = Expect(SyntaxTokenType.VarKeyword);

            var name = Expect(SyntaxTokenType.Identifier);

            TypeClause? typeClause = null;

            if (current.Type == SyntaxTokenType.Colon)
                typeClause = ParseTypeClause();

            var equals = Expect(SyntaxTokenType.Equals);
            var expr = ParseExpressionImpl();
            var semi = Expect(SyntaxTokenType.SemiColon);
            return new DeclarationStatement(decl, name, typeClause, equals, expr, semi);
        }

        private SyntaxStatement ParseExpressionStatement()
        {
            var expr = ParseExpressionImpl();
            var semi = Expect(SyntaxTokenType.SemiColon);
            return new ExpressionStatement(expr, semi);
        }

        private SyntaxExpression ParseExpressionImpl()
        {
            return ParseAssignmentExpression();
        }

        private SyntaxExpression ParseAssignmentExpression()
        {
            var left = ParseBinaryExpression();

            if (current.Type.IsAssignmentOperator())
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

            while (current.Type.BinaryOperatorPrecedence() == precedence)
            {
                var op = Advance();
                var right = ParseBinaryExpression(precedence - 1);
                left = new BinaryExpression(left, op, right);
            }

            return left;
        }

        private SyntaxExpression ParsePrefixExpression()
        {
            if (current.Type.IsPrefixOperator())
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

            while (current.Type.IsPostfixOperator())
            {
                if (current.Type == SyntaxTokenType.LeftParen)
                {
                    var lparen = Expect(SyntaxTokenType.LeftParen);

                    var arguments = ArgumentExpression.Empty;

                    if (current.Type != SyntaxTokenType.RightParen)
                        arguments = ParseArguments();

                    var rparen = Expect(SyntaxTokenType.RightParen);
                    node = new CallExpression(node, lparen, arguments, rparen);
                }
                else if (current.Type == SyntaxTokenType.LeftSquare)
                {
                    var lsquare = Expect(SyntaxTokenType.LeftSquare);
                    var expr = ParseExpressionImpl();
                    var rsqare = Expect(SyntaxTokenType.RightSquare);

                    node = new SubscriptExpression(node, lsquare, expr, rsqare);
                }
                else if (current.Type == SyntaxTokenType.Dot)
                {
                    var dotToken = Advance();
                    var nameToken = Expect(SyntaxTokenType.Identifier);
                    node = new MemberAccessExpression(node, dotToken, new IdentifierExpression(nameToken));
                }
            }

            return node;
        }

        private SyntaxExpression ParsePrimary()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralExpression(Advance());
            }
            else if (current.Type == SyntaxTokenType.LeftParen)
            {
                var leftParen = Expect(SyntaxTokenType.LeftParen);
                var expr = ParseExpressionImpl();
                var rightParen = Expect(SyntaxTokenType.RightParen);

                return new ParenthesizedExpression(leftParen, expr, rightParen);
            }
            else if (current.Type == SyntaxTokenType.Identifier)
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