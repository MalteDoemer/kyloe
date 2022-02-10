using Kyloe.Text;

namespace Kyloe.Syntax
{

    class ParenthesizedExpressionNode : SyntaxNode
    {
        public ParenthesizedExpressionNode(SyntaxToken leftParen, SyntaxToken rightParen, SyntaxNode expression)
        {
            LeftParen = leftParen;
            RightParen = rightParen;
            Expression = expression;
        }

        public SyntaxToken LeftParen { get; }
        public SyntaxToken RightParen { get; }
        public SyntaxNode Expression { get; }

        public override SourceLocation Location => SourceLocation.CreateAround(LeftParen.Location, RightParen.Location);
    }

}