

using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class MemberAccessExpression : SyntaxNode
    {
        public MemberAccessExpression(SyntaxNode expression, SyntaxToken dotToken, SyntaxToken nameToken)
        {
            Expression = expression;
            NameToken = nameToken;
            DotToken = dotToken;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken NameToken { get; }
        public SyntaxToken DotToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.MemberAccessExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, NameToken.Location);
    }

}