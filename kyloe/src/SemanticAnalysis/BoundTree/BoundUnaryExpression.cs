using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression expression, UnaryOperation operation, ISymbol result)
        {
            Expression = expression;
            Operation = operation;
            ResultSymbol = result;
        }

        public BoundExpression Expression { get; }
        public UnaryOperation Operation { get; }

        public override ISymbol ResultSymbol { get; }

        public override BoundNodeType Type => BoundNodeType.BoundUnaryExpression;

        public override bool IsValue => true;
    }
}