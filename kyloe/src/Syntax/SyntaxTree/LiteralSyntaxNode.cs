using Kyloe.Text;

namespace Kyloe.Syntax
{
    class LiteralSyntaxNode : SyntaxNode
    {
        public LiteralSyntaxNode(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public SyntaxToken LiteralToken { get; }

        public override SourceLocation Location => LiteralToken.Location;
    }
}