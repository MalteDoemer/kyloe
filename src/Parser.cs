using System.Collections.Generic;
using System.IO;

namespace Kyloe
{
    abstract class SyntaxNode
    {
    }

    class MalformedSyntaxNode : SyntaxNode
    {
        public MalformedSyntaxNode(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }


    }

    class LiteralSyntaxNode : SyntaxNode
    {
        public LiteralSyntaxNode(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public SyntaxToken LiteralToken { get; }

    }

    class UnaryExpressionNode : SyntaxNode
    {
        public UnaryExpressionNode(SyntaxToken operatorToken, SyntaxNode child)
        {
            OperatorToken = operatorToken;
            Child = child;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode Child { get; }

    }

    class BinaryExpressionNode : SyntaxNode
    {
        public BinaryExpressionNode(SyntaxToken operatorToken, SyntaxNode leftChild, SyntaxNode rightChild)
        {
            OperatorToken = operatorToken;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode LeftChild { get; }
        public SyntaxNode RightChild { get; }

    }

    class Parser
    {
        private Lexer lexer;

        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(string text)
        {
            lexer = new Lexer(text);
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
            foreach (var type in types)
            {
                if (current.Type == type)
                    return Advance();
            }

            throw new System.NotImplementedException();
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
                return new MalformedSyntaxNode(Advance());
            }
        }
    }
}