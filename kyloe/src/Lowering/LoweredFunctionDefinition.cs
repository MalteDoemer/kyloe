using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredFunctionDefinition : LoweredNode 
    {
        public LoweredFunctionDefinition(FunctionType type, LoweredStatement body)
        {
            Type = type;
            Body = body;
        }

        public FunctionType Type { get; }
        public LoweredStatement Body { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredFunctionDefinition;
    }
}
