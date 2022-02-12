using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class NameExpression : SyntaxNode
    {
        public NameExpression(SyntaxToken nameToken)
        {
            NameToken = nameToken;
        }

        public SyntaxToken NameToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.NameExpression;

        public override SourceLocation Location => NameToken.Location;
    }

}