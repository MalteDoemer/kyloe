using System.Collections.Generic;

namespace Kyloe
{
    abstract class ASTNode
    {

    }

    class BinaryExpressionNode : ASTNode
    {
        public BinaryExpressionNode(SyntaxToken operatorToken, ASTNode leftChild, ASTNode rightChild)
        {
            OperatorToken = operatorToken;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public SyntaxToken OperatorToken { get; }
        public ASTNode LeftChild { get; }
        public ASTNode RightChild { get; }
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

        public ASTNode Parse()
        {
            throw new System.NotImplementedException();
        }
    }
}