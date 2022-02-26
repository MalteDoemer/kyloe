namespace Kyloe.Semantics
{
    internal class BoundBinaryExpression : BoundExpression
    {
        private readonly BoundResultType resultType;

        public BoundBinaryExpression(BoundExpression leftChild, BinaryOperation operation, BoundExpression rightChild, BoundResultType resultType)
        {
            LeftChild = leftChild;
            Operation = operation;
            RightChild = rightChild;
            this.resultType = resultType;
        }

        public BoundExpression LeftChild { get; }
        public BinaryOperation Operation { get; }
        public BoundExpression RightChild { get; }

        public override BoundResultType Result => resultType;

        public override BoundNodeType Type => BoundNodeType.BoundBinaryExpression;
    }
}