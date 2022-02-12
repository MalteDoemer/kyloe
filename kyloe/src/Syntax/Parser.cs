using System;
using System.Collections.Immutable;
using Kyloe.Diagnostics;

namespace Kyloe.Syntax
{

    class Parser
    {
        private Lexer lexer;

        private DiagnosticCollecter diagnostics;

        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(string text, DiagnosticCollecter diagnostics)
        {
            this.diagnostics = diagnostics;
            lexer = new Lexer(text, diagnostics);
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
            if (types.Length == 0)
                throw new ArgumentException("There must be at least one type");

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
            var expr = ParseExpression();
            Expect(SyntaxTokenType.End);
            return expr;
        }

        public SyntaxNode ParseExpression()
        {
            return ParseBinaryExpression();
        }

        private SyntaxNode ParseBinaryExpression(int precedence = SyntaxInfo.MAX_PRECEDENCE)
        {
            if (precedence == 0)
                return ParsePrefixExpression();

            var left = ParseBinaryExpression(precedence - 1);

            while (current.Type.BinaryOperatorPrecedence() == precedence)
            {
                var op = Advance();
                var right = ParseBinaryExpression(precedence - 1);
                left = new BinaryExpressionNode(op, left, right);
            }

            return left;
        }

        private SyntaxNode ParsePrefixExpression()
        {
            if (current.Type.IsPrefixOperator())
            {
                var op = Advance();
                var child = ParsePrefixExpression();
                return new UnaryExpressionNode(op, child);
            }

            return ParsePostFixExpression();
        }

        private SyntaxNode ParsePostFixExpression()
        {
            var node = ParsePrimary();

            while (current.Type.IsPostfixOperator())
            {
                if (current.Type == SyntaxTokenType.LeftParen)
                {
                    var lparen = Advance();

                    ArgumentNode? arguments = null;

                    if (current.Type != SyntaxTokenType.RightParen)
                        arguments = ParseArguments();

                    var rparen = Expect(SyntaxTokenType.RightParen);
                    node = new CallExpressionNode(node, lparen, arguments, rparen);
                }
                else if (current.Type == SyntaxTokenType.LeftSquare)
                {
                    var lsquare = Advance();
                    var expr = ParseExpression();
                    var rsqare = Expect(SyntaxTokenType.RightSquare);

                    node = new SubscriptExpressionNode(node, lsquare, expr, rsqare);
                }
                else if (current.Type == SyntaxTokenType.Dot)
                {
                    var dotToken = Advance();
                    var nameToken = Expect(SyntaxTokenType.Identifier);
                    node = new MemberAccessNode(node, dotToken, nameToken);
                }
            }

            return node;
        }

        private ArgumentNode ParseArguments()
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

            return new ArgumentNode(nodes.ToImmutable(), commas.ToImmutable());
        }

        private SyntaxNode ParsePrimary()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralSyntaxNode(Advance());
            }
            else if (current.Type == SyntaxTokenType.LeftParen)
            {
                var leftParen = Advance(); // skip the left parenthesis
                var expr = ParseExpression();
                var rightParen = Expect(SyntaxTokenType.RightParen); // skip the right parenthesis

                return new ParenthesizedExpressionNode(leftParen, rightParen, expr);
            }
            else if (current.Type == SyntaxTokenType.Identifier)
            {
                var name = Advance();
                return new NameExpressionNode(name);
            }
            else
            {   // Don't report a new diagnostic if the lexer already did.
                if (current.Type != SyntaxTokenType.Invalid)
                    diagnostics.Add(new UnexpectedTokenError(current));

                return new MalformedSyntaxNode(Advance());
            }
        }
    }
}