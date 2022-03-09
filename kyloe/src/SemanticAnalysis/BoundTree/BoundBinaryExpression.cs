using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression leftExpression, BinaryOperation operation, BoundExpression rightExpression, ITypeSymbol result)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = result;
        }

        public ITypeSymbol ResultType { get; }
        public BoundExpression LeftExpression { get; }
        public BinaryOperation Operation { get; }
        public BoundExpression RightExpression { get; }


        public override ISymbol ResultSymbol => ResultType;

        public override BoundNodeType Type => BoundNodeType.BoundBinaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.RValue;
    }
}