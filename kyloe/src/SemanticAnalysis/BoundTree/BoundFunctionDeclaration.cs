using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDeclaration : BoundNode
    {
        public BoundFunctionDeclaration(FunctionType type, BoundParameters parameters, BoundTypeClause typeClause)
        {
            Type = type;
            Parameters = parameters;
            TypeClause = typeClause;
        }

        public FunctionType Type { get; }
        public BoundParameters Parameters { get; }
        public BoundTypeClause TypeClause { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDeclaration;
    }
}