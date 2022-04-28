using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDefinition : BoundNode
    {
        public BoundFunctionDefinition(FunctionType functionType, BoundBlockStatement body)
        {
            FunctionType = functionType;
            Body = body;
        }

        public FunctionType FunctionType { get; }
        public BoundBlockStatement Body { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDefinition;
    }
}