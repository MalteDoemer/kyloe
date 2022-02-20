using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class UnaryExpression: SyntaxExpression
    {
        public UnaryExpression(SyntaxToken operatorToken, SyntaxNode expression)
        {
            OperatorToken = operatorToken;
            Expression = expression;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode Expression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.UnaryExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(OperatorToken.Location, Expression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(Expression);
        }
    }
}