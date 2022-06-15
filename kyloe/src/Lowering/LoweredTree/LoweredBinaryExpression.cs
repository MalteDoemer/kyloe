using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredBinaryExpression : LoweredExpression 
    {
        public LoweredBinaryExpression(LoweredExpression leftExpression, BoundOperation operation, LoweredExpression rightExpression, MethodType method)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            Method = method;
        }

        public LoweredExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public LoweredExpression RightExpression { get; }
        public MethodType Method { get; }

        public override TypeInfo Type => Method.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredBinaryExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return LeftExpression;
            yield return RightExpression;
        }
    }
}
