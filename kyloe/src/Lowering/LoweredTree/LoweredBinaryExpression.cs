using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredBinaryExpression : LoweredExpression 
    {
        public LoweredBinaryExpression(TypeSpecifier type, LoweredExpression leftExpression, BoundOperation operation, LoweredExpression rightExpression)
        {
            Type = type;
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
        }

        public LoweredExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public LoweredExpression RightExpression { get; }

        public override TypeSpecifier Type { get; }

        public override bool HasKnownValue => LeftExpression.HasKnownValue && RightExpression.HasKnownValue;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredBinaryExpression;
    }
}
