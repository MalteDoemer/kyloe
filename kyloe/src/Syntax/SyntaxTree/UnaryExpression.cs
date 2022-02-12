using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class UnaryExpression : SyntaxNode
    {
        public UnaryExpression(SyntaxToken operatorToken, SyntaxNode child)
        {
            OperatorToken = operatorToken;
            Child = child;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode Child { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.UnaryExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(OperatorToken.Location, Child.Location);
    }
}