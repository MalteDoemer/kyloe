using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class ParenthesizedExpression : SyntaxExpression
    {
        public ParenthesizedExpression(SyntaxToken leftParen, SyntaxExpression expression, SyntaxToken rightParen)
        {
            LeftParen = leftParen;
            Expression = expression;
            RightParen = rightParen;
        }

        public SyntaxToken LeftParen { get; }
        public SyntaxExpression Expression { get; }
        public SyntaxToken RightParen { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ParenthesizedExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftParen.Location, RightParen.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftParen);
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(RightParen);
        }
    }

}