using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredBinaryExpression : LoweredExpression 
    {
        public LoweredBinaryExpression(TypeInfo type, LoweredExpression leftExpression, BoundOperation operation, LoweredExpression rightExpression)
        {
            Type = type;
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
        }

        public LoweredExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public LoweredExpression RightExpression { get; }

        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredBinaryExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return LeftExpression;
            yield return RightExpression;
        }
    }
}
