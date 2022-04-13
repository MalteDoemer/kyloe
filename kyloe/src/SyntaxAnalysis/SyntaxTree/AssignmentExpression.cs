using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class AssignmentExpression : SyntaxExpression
    {
        public AssignmentExpression(SyntaxExpression leftNode, SyntaxToken operatorToken, SyntaxExpression rightNode)
        {
            LeftNode = leftNode;
            OperatorToken = operatorToken;
            RightNode = rightNode;
        }

        public SyntaxExpression LeftNode { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxExpression RightNode { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.AssignmentExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftNode.Location, RightNode.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftNode);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightNode);
        }
    }
}