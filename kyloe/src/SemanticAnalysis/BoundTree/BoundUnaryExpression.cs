using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression expression, BoundOperation operation, ITypeSymbol result)
        {
            Expression = expression;
            Operation = operation;
            ResultType = result;
        }

        public BoundExpression Expression { get; }
        public BoundOperation Operation { get; }
        
        public override ITypeSymbol ResultType { get; }

        public override BoundNodeType Type => BoundNodeType.BoundUnaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.RValue;
    }
}