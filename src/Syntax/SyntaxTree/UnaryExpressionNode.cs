namespace Kyloe.Syntax
{
    class UnaryExpressionNode : SyntaxNode
    {
        public UnaryExpressionNode(SyntaxToken operatorToken, SyntaxNode child)
        {
            OperatorToken = operatorToken;
            Child = child;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode Child { get; }

    }
}