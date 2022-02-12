

using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class MemberAccessExpression : SyntaxNode
    {
        public MemberAccessExpression(SyntaxNode expression, SyntaxToken dotToken, SyntaxToken nameToken)
        {
            Expression = expression;
            DotToken = dotToken;
            NameToken = nameToken;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken DotToken { get; }
        public SyntaxToken NameToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.MemberAccessExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, NameToken.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(DotToken);
            yield return new SyntaxNodeChild(NameToken);
        }
    }

}