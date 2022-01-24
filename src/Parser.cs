using System.Collections.Generic;

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
            return current;
        }

        public SyntaxNode Parse()
        {
            return ParseExpression();
        }

        private SyntaxNode ParseExpression()
        {
            return ParsePrimary();
        }

        private SyntaxNode ParsePrimary()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralSyntaxNode(Advance());
            }
            else if (current.Type == SyntaxTokenType.RightParen)
            {
                return ParseExpression();
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