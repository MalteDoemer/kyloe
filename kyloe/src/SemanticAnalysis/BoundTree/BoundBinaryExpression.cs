using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression leftExpression, BinaryOperation operation, BoundExpression rightExpression, ISymbol result)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultSymbol = result;
        }

        public BoundExpression LeftExpression { get; }
        public BinaryOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override ISymbol ResultSymbol { get; }

        public override BoundNodeType Type => BoundNodeType.BoundBinaryExpression;

        public override bool IsValue => true;
    }
}