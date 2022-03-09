using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression expression, UnaryOperation operation, ITypeSymbol result)
        {
            Expression = expression;
            Operation = operation;
            ResultType = result;
        }

        public BoundExpression Expression { get; }
        public UnaryOperation Operation { get; }
        public ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

        public override BoundNodeType Type => BoundNodeType.BoundUnaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.RValue;
    }
}