using System.IO;
using System.Diagnostics;
using System.Collections.Immutable;
using Kyloe.Diagnostics;

namespace Kyloe.Syntax
{
    internal class Parser
    {
        private Lexer lexer;

        private DiagnosticCollector diagnostics;

        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(Lexer lexer, DiagnosticCollector diagnostics)
        {
            this.diagnostics = diagnostics;
            this.lexer = lexer;
            current = lexer.NextToken();
            next = lexer.NextToken();
        }

        /// Returns the current Token and then advances to the next one.
        private SyntaxToken Advance()
        {
            var temp = current;
            current = next;
            next = lexer.NextToken();
            return temp;
        }

        private SyntaxToken Expect(params SyntaxTokenType[] types)
        {
            Debug.Assert(types.Length != 0, "There must be at least one type to expect");

            foreach (var type in types)
            {
                if (current.Type == type)
                    return Advance();
            }

            // Don't report a new diagnostic if the lexer already did.
            if (current.Type != SyntaxTokenType.Invalid)
                diagnostics.Add(new UnexpectedTokenError(types, current));

            return Advance();
        }

        public SyntaxNode Parse()
        {
            var stmt = ParseStatement();
            Expect(SyntaxTokenType.End);
            return stmt;
        }

        public SyntaxStatement ParseStatement()
        {
            switch (current.Type)
            {
                default:
                    var expr = ParseExpression();
                    var semi = Expect(SyntaxTokenType.SemiColon);
                    return new ExpressionStatement(expr, semi);
            }
        }

        public SyntaxExpression ParseExpression()
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
                var child = ParsePrefixExpression();
                return new UnaryExpression(op, child);
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
                    var lparen = Advance();

                    ArgumentExpression? arguments = null;

                    if (current.Type != SyntaxTokenType.RightParen)
                        arguments = ParseArguments();

                    var rparen = Expect(SyntaxTokenType.RightParen);
                    node = new CallExpression(node, lparen, arguments, rparen);
                }
                else if (current.Type == SyntaxTokenType.LeftSquare)
                {
                    var lsquare = Advance();
                    var expr = ParseExpression();
                    var rsqare = Expect(SyntaxTokenType.RightSquare);

                    node = new SubscriptExpression(node, lsquare, expr, rsqare);
                }
                else if (current.Type == SyntaxTokenType.Dot)
                {
                    var dotToken = Advance();
                    var nameToken = Expect(SyntaxTokenType.Identifier);
                    node = new MemberAccessExpression(node, dotToken, new NameExpression(nameToken));
                }
            }

            return node;
        }

        private ArgumentExpression ParseArguments()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();
            var commas = ImmutableArray.CreateBuilder<SyntaxToken>();

            while (true)
            {
                nodes.Add(ParseExpression());

                if (current.Type != SyntaxTokenType.Comma)
                    break;

                commas.Add(Advance());
            }

            return new ArgumentExpression(nodes.ToImmutable(), commas.ToImmutable());
        }

        private SyntaxExpression ParsePrimary()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralExpression(Advance());
            }
            else if (current.Type == SyntaxTokenType.LeftParen)
            {
                var leftParen = Advance(); // skip the left parenthesis
                var expr = ParseExpression();
                var rightParen = Expect(SyntaxTokenType.RightParen); // skip the right parenthesis

                return new ParenthesizedExpression(leftParen, rightParen, expr);
            }
            else if (current.Type == SyntaxTokenType.Identifier)
            {
                var name = Advance();
                return new NameExpression(name);
            }
            else
            {
                // Don't report a new diagnostic if the lexer already did.
                if (current.Type != SyntaxTokenType.Invalid)
                    diagnostics.Add(new UnexpectedTokenError(current));

                return new MalformedExpression(Advance());
            }
        }
    }
}