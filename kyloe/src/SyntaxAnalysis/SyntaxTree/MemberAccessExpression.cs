using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class MemberAccessExpression : SyntaxExpression
    {
        public MemberAccessExpression(SyntaxExpression expression, SyntaxToken dotToken, IdentifierExpression identifierExpression)
        {
            Expression = expression;
            DotToken = dotToken;
            IdentifierExpression = identifierExpression;
        }

        public SyntaxExpression Expression { get; }
        public SyntaxToken DotToken { get; }
        public IdentifierExpression IdentifierExpression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.MemberAccessExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, IdentifierExpression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(DotToken);
            yield return new SyntaxNodeChild(IdentifierExpression);
        }
    }
}