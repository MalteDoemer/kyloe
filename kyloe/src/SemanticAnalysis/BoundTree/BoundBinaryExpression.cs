using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression leftExpression, BoundOperation operation, BoundExpression rightExpression, ITypeSymbol result)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = result;
        }

        public BoundExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

        public override BoundNodeType Type => BoundNodeType.BoundBinaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}