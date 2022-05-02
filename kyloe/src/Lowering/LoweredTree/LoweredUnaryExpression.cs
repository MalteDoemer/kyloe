using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredUnaryExpression : LoweredExpression 
    {
        public LoweredUnaryExpression(TypeSpecifier type, LoweredExpression Expression, BoundOperation operation)
        {
            Type = type;
            this.Expression = Expression;
            Operation = operation;
        }

        public LoweredExpression Expression { get; }
        public BoundOperation Operation { get; }

        public override TypeSpecifier Type { get; }

        public override bool HasKnownValue => Expression.HasKnownValue;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredUnaryExpression;
    }
}
