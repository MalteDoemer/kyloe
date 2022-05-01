using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDefinition : BoundNode
    {
        public BoundFunctionDefinition(BoundFunctionDeclaration declaration, BoundBlockStatement body)
        {
            Declaration = declaration;
            Body = body;
        }

        public BoundFunctionDeclaration Declaration { get; }
        public BoundBlockStatement Body { get; }

        public FunctionType Type => Declaration.Type;

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDefinition;
    }
}