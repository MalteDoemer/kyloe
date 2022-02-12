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

        public override SourceLocation Location => NameToken.Location;
    }

}