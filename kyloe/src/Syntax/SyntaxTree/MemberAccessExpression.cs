

using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class MemberAccessExpression : SyntaxNode
    {
        public MemberAccessExpression(SyntaxNode expression, SyntaxToken dotToken, NameExpression nameExpression)
        {
            Expression = expression;
            DotToken = dotToken;
            NameExpression = nameExpression;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken DotToken { get; }
        public NameExpression NameExpression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.MemberAccessExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, NameExpression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(DotToken);
            yield return new SyntaxNodeChild(NameExpression);
        }
    }

}