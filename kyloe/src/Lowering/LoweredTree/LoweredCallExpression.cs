using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredCallExpression : LoweredExpression
    {
        public LoweredCallExpression(FunctionType functionType, LoweredExpression expression, LoweredArguments arguments)
        {
            FunctionType = functionType;
            Expression = expression;
            Arguments = arguments;
        }

        public FunctionType FunctionType { get; }
        public LoweredExpression Expression { get; }
        public LoweredArguments Arguments { get; }

        public override TypeSpecifier Type => FunctionType.ReturnType;

        public override bool HasKnownValue => false;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredCallExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Expression;
            yield return Arguments;
        }
    }
}
