namespace Kyloe.Semantics
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundExpressionStatement;
    }
}