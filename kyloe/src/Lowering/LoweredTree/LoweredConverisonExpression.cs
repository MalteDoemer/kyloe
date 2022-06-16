using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredConversionExpression : LoweredExpression
    {
        public LoweredConversionExpression(LoweredExpression expression, MethodType method)
        {
            Expression = expression;
            Method = method;
        }

        public LoweredExpression Expression { get; }
        public MethodType Method { get; }
        
        public override TypeInfo Type => Method.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredConversionExpression;


        public override IEnumerable<LoweredNode> Children()
        {
            yield return Expression;
        }
    }
}
