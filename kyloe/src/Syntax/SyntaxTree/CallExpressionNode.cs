using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class CallExpressionNode : SyntaxNode
    {
        public CallExpressionNode(SyntaxNode expression, SyntaxToken leftParen, ArgumentNode? arguments, SyntaxToken rightParen)
        {
            Expression = expression;
            LeftParen = leftParen;
            Arguments = arguments;
            RightParen = rightParen;
        }

        public SyntaxNode Expression { get; }
        public SyntaxToken LeftParen { get; }
        public ArgumentNode? Arguments { get; }
        public SyntaxToken RightParen { get; }

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, RightParen.Location);
    }


}