using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredCallExpression : LoweredExpression
    {
        public LoweredCallExpression(CallableType callableType, LoweredExpression expression, LoweredArguments arguments)
        {
            CallableType = callableType;
            Expression = expression;
            Arguments = arguments;
        }

        public CallableType CallableType { get; }
        public LoweredExpression Expression { get; }
        public LoweredArguments Arguments { get; }

        public override TypeInfo Type => CallableType.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredCallExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Expression;
            yield return Arguments;
        }
    }
}
