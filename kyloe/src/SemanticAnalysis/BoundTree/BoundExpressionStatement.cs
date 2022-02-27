namespace Kyloe.Semantics
{
    internal class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; }

        public override BoundNodeType Type => BoundNodeType.BoundExpressionStatement;
    }
}