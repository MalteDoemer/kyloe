using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class NameExpressionNode : SyntaxNode
    {
        public NameExpressionNode(SyntaxToken nameToken)
        {
            NameToken = nameToken;
        }

        public SyntaxToken NameToken { get; }

        public override SourceLocation Location => NameToken.Location;
    }

}