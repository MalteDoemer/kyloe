using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class LiteralExpression : SyntaxNode
    {
        public LiteralExpression(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public SyntaxToken LiteralToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.LiteralExpression;

        public override SourceLocation Location => LiteralToken.Location;
    }
}