using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class ParenthesizedExpression: SyntaxExpression
    {
        public ParenthesizedExpression(SyntaxToken leftParen, SyntaxToken rightParen, SyntaxNode expression)
        {
            LeftParen = leftParen;
            RightParen = rightParen;
            Expression = expression;
        }

        public SyntaxToken LeftParen { get; }
        public SyntaxToken RightParen { get; }
        public SyntaxNode Expression { get; }

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