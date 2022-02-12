using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class BinaryExpression : SyntaxNode
    {
        public BinaryExpression(SyntaxNode leftChild, SyntaxToken operatorToken, SyntaxNode rightChild)
        {
            LeftChild = leftChild;
            OperatorToken = operatorToken;
            RightChild = rightChild;
        }

        public SyntaxNode LeftChild { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxNode RightChild { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.BinaryExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftChild.Location, RightChild.Location);

        internal override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftChild);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightChild);
        }
    }
}