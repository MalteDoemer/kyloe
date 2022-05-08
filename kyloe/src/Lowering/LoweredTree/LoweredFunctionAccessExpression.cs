using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredFunctionAccessExpression : LoweredExpression
    {
        public LoweredFunctionAccessExpression(FunctionGroupSymbol functionGroup)
        {
            FunctionGroup = functionGroup;
        }

        public FunctionGroupSymbol FunctionGroup { get; }

        public override TypeInfo Type => FunctionGroup.Type;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredFunctionAccessExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
