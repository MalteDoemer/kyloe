using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression leftExpression, BoundOperation operation, BoundExpression rightExpression, TypeSpecifier result)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = result;
        }

        public BoundExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override TypeSpecifier ResultType { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundBinaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}