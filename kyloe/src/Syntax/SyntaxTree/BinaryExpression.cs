using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class BinaryExpression : SyntaxNode
    {
        public BinaryExpression(SyntaxToken operatorToken, SyntaxNode leftChild, SyntaxNode rightChild)
        {
            OperatorToken = operatorToken;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode LeftChild { get; }
        public SyntaxNode RightChild { get; }

        public override SourceLocation Location => SourceLocation.CreateAround(LeftChild.Location, RightChild.Location);
    }
}