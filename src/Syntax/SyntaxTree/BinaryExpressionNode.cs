namespace Kyloe.Syntax
{
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
}