using Kyloe.Utility;

namespace Kyloe.Syntax
{


    class SubscriptExpression : SyntaxNode
    {
        public SubscriptExpression(SyntaxNode leftNode, SyntaxToken leftSquare, SyntaxNode subscript, SyntaxToken rightSquare)
        {
            LeftNode = leftNode;
            LeftSquare = leftSquare;
            Subscript = subscript;
            RightSquare = rightSquare;
        }

        public SyntaxNode LeftNode { get; }
        public SyntaxToken LeftSquare { get; }
        public SyntaxNode Subscript { get; }
        public SyntaxToken RightSquare { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.SubscriptExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftNode.Location, RightSquare.Location);
    }

}