using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class BinaryExpression : SyntaxExpression
    {
        public BinaryExpression(SyntaxExpression leftChild, SyntaxToken operatorToken, SyntaxExpression rightChild)
        {
            LeftChild = leftChild;
            OperatorToken = operatorToken;
            RightChild = rightChild;
        }

        public SyntaxExpression LeftChild { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxExpression RightChild { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.BinaryExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftChild.Location, RightChild.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftChild);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightChild);
        }
    }
}