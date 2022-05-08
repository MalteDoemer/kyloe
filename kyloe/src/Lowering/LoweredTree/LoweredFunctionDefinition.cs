using System.Collections.Generic;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredFunctionDefinition : LoweredNode 
    {
        public LoweredFunctionDefinition(FunctionTypeInfo functionType, LoweredBlockStatement body)
        {
            FunctionType = functionType;
            Body = body;
        }

        public FunctionTypeInfo FunctionType { get; }
        public LoweredBlockStatement Body { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredFunctionDefinition;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Body;
        }
    }
}
