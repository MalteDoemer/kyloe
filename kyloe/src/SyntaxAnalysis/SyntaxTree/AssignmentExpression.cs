using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class AssignmentSyntax : SyntaxNode
    {
        public AssignmentSyntax(SyntaxNode leftNode, SyntaxToken operatorToken, SyntaxNode rightNode)
        {
            LeftNode = leftNode;
            OperatorToken = operatorToken;
            RightNode = rightNode;
        }

        public SyntaxNode LeftNode { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxNode RightNode { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.AssignmentSyntax;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftNode.Location, RightNode.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftNode);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightNode);
        }
    }
}