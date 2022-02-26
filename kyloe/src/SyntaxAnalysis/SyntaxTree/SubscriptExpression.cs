using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{


    internal class SubscriptExpression: SyntaxExpression
    {
        public SubscriptExpression(SyntaxExpression leftNode, SyntaxToken leftSquare, SyntaxExpression subscript, SyntaxToken rightSquare)
        {
            LeftNode = leftNode;
            LeftSquare = leftSquare;
            Subscript = subscript;
            RightSquare = rightSquare;
        }

        public SyntaxExpression LeftNode { get; }
        public SyntaxToken LeftSquare { get; }
        public SyntaxExpression Subscript { get; }
        public SyntaxToken RightSquare { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.SubscriptExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftNode.Location, RightSquare.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftNode);
            yield return new SyntaxNodeChild(LeftSquare);
            yield return new SyntaxNodeChild(Subscript);
            yield return new SyntaxNodeChild(RightSquare);
        }
    }

}