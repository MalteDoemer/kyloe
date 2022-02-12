using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class ParenthesizedExpression : SyntaxNode
    {
        public ParenthesizedExpression(SyntaxToken leftParen, SyntaxToken rightParen, SyntaxNode expression)
        {
            LeftParen = leftParen;
            RightParen = rightParen;
            Expression = expression;
        }

        public SyntaxToken LeftParen { get; }
        public SyntaxToken RightParen { get; }
        public SyntaxNode Expression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ParenthesizedExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftParen.Location, RightParen.Location);
    }

}