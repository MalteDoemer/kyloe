using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class CallExpression : SyntaxNode
    {
        public CallExpression(SyntaxNode expression, SyntaxToken leftParen, ArgumentExpression? arguments, SyntaxToken rightParen)
        {
            Expression = expression;
            LeftParen = leftParen;
            Arguments = arguments;
            RightParen = rightParen;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken LeftParen { get; }
        public ArgumentExpression? Arguments { get; }
        public SyntaxToken RightParen { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.CallExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, RightParen.Location);
    }


}