using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class CallExpression: SyntaxExpression
    {
        public CallExpression(SyntaxNode expression, SyntaxToken leftParen, ArgumentExpression arguments, SyntaxToken rightParen)
        {
            Expression = expression;
            LeftParen = leftParen;
            Arguments = arguments;
            RightParen = rightParen;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken LeftParen { get; }
        public ArgumentExpression Arguments { get; }
        public SyntaxToken RightParen { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.CallExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, RightParen.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(LeftParen);

            foreach (var child in Arguments.GetChildren())
                yield return child;

            yield return new SyntaxNodeChild(RightParen);
        }
    }


}