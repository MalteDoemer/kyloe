namespace Kyloe.Semantics
{
    internal class BoundBinaryExpression : BoundExpression
    {
        private readonly BoundResultType resultType;

        public BoundBinaryExpression(BoundExpression leftExpression, BinaryOperation operation, BoundExpression rightExpression, BoundResultType resultType)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            this.resultType = resultType;
        }

        public BoundExpression LeftExpression { get; }
        public BinaryOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override BoundResultType Result => resultType;

        public override BoundNodeType Type => BoundNodeType.BoundBinaryExpression;
    }
}