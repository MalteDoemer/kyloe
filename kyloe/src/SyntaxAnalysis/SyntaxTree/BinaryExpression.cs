using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class BinarySyntax : SyntaxNode
    {
        public BinarySyntax(SyntaxNode leftExpression, SyntaxToken operatorToken, SyntaxNode rightExpression)
        {
            LeftExpression = leftExpression;
            OperatorToken = operatorToken;
            RightExpression = rightExpression;
        }

        public SyntaxNode LeftExpression { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxNode RightExpression { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.BinarySyntax;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftExpression.Location, RightExpression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftExpression);
            yield return new SyntaxNodeChild(OperatorToken);
            yield return new SyntaxNodeChild(RightExpression);
        }
    }
}