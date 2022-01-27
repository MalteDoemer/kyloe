using System;
using System.IO;
using System.Collections.Generic;

using Kyloe.Diagnostics;

namespace Kyloe.Syntax
{

    class Parser
    {
        private Lexer lexer;

        private List<Diagnostic> diagnostics;

        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(string text)
        {
            diagnostics = new List<Diagnostic>();
            lexer = new Lexer(text);
            current = lexer.NextToken();
            next = lexer.NextToken();
        }

        public IEnumerable<Diagnostic> GetDiagnostics() => diagnostics;

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

            diagnostics.Add(new UnexpectedTokenError(types, current.Type));

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
                return ParseUnaryExpression();

            var left = ParseBinaryExpression(precedence - 1);

            while (current.Type.BinaryOperatorPrecedence() == precedence)
            {
                var op = Advance();
                var right = ParseBinaryExpression(precedence - 1);
                left = new BinaryExpressionNode(op, left, right);
            }

            return left;
        }

        private SyntaxNode ParseUnaryExpression()
        {
            if (current.Type.IsUnaryOperator())
            {
                var op = Advance();
                var child = ParseUnaryExpression();
                return new UnaryExpressionNode(op, child);
            }

            return ParsePrimary();
        }

        private SyntaxNode ParsePrimary()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralSyntaxNode(Advance());
            }
            else if (current.Type == SyntaxTokenType.LeftParen)
            {
                Advance(); // skip the left parenthesis
                var expr = ParseExpression();
                Expect(SyntaxTokenType.RightParen); // skip the right parenthesis
                return expr;
            }
            else if (current.Type == SyntaxTokenType.Identifier)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                Console.WriteLine("hi");
                diagnostics.Add(new UnexpectedTokenError(current.Type));
                return new MalformedSyntaxNode(Advance());
            }
        }
    }
}