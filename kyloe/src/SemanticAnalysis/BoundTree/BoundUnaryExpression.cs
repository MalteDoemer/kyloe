namespace Kyloe.Semantics
{
    internal class BoundUnaryExpression : BoundExpression
    {
        private readonly BoundResultType resultType;

        public BoundUnaryExpression(BoundExpression expression, UnaryOperation operation, BoundResultType resultType)
        {
            Expression = expression;
            Operation = operation;
            this.resultType = resultType;
        }

        public BoundExpression Expression { get; }
        public UnaryOperation Operation { get; }

        public override BoundResultType Result => resultType;

        public override BoundNodeType Type => BoundNodeType.BoundUnaryExpression;
    }
}