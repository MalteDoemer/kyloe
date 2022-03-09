using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class BinaryExpression : SyntaxExpression
    {
        public BinaryExpression(SyntaxExpression leftExpression, SyntaxToken operatorToken, SyntaxExpression rightExpression)
        {
            LeftExpression = leftExpression;
            OperatorToken = operatorToken;
            RightExpression = rightExpression;
        }

        public SyntaxExpression LeftExpression { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxExpression RightExpression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.BinaryExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftExpression.Location, RightExpression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftExpression);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightExpression);
        }
    }
}