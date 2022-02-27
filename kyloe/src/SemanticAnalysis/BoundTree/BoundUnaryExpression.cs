namespace Kyloe.Semantics
{
    internal class BoundUnaryExpression : BoundExpression
    {
        private readonly BoundResultType resultType;

        public BoundUnaryExpression(BoundExpression child, UnaryOperation operation, BoundResultType resultType)
        {
            Child = child;
            Operation = operation;
            this.resultType = resultType;
        }

        public BoundExpression Child { get; }
        public UnaryOperation Operation { get; }

        public override BoundResultType Result => resultType;

        public override BoundNodeType Type => BoundNodeType.BoundUnaryExpression;
    }
}