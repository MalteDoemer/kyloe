using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression expression, BoundOperation operation, TypeSpecifier result)
        {
            Expression = expression;
            Operation = operation;
            ResultType = result;
        }

        public BoundExpression Expression { get; }
        public BoundOperation Operation { get; }
        
        public override TypeSpecifier ResultType { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundUnaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}