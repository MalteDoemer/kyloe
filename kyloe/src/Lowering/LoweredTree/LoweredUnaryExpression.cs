using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredUnaryExpression : LoweredExpression 
    {
        public LoweredUnaryExpression(TypeInfo type, LoweredExpression Expression, BoundOperation operation)
        {
            Type = type;
            this.Expression = Expression;
            Operation = operation;
        }

        public LoweredExpression Expression { get; }
        public BoundOperation Operation { get; }

        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredUnaryExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Expression;
        }
    }
}
