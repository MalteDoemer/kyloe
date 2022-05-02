using System.Collections.Generic;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredFunctionDefinition : LoweredNode 
    {
        public LoweredFunctionDefinition(FunctionType type, LoweredBlockStatement body)
        {
            Type = type;
            Body = body;
        }

        public FunctionType Type { get; }
        public LoweredBlockStatement Body { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredFunctionDefinition;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Body;
        }
    }
}
