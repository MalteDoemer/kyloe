using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredUnaryExpression : LoweredExpression 
    {
        public LoweredUnaryExpression(LoweredExpression Expression, BoundOperation operation, MethodType method)
        {
            this.Expression = Expression;
            Operation = operation;
            Method = method;
        }

        public LoweredExpression Expression { get; }
        public BoundOperation Operation { get; }
        public MethodType Method { get; }
        
        public override TypeInfo Type => Method.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredUnaryExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Expression;
        }
    }
}
