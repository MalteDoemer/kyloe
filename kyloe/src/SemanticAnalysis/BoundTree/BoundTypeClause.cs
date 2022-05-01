using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeClause : BoundNode
    {
        public BoundTypeClause(TypeSpecifier type)
        {
            Type = type;
        }

        public TypeSpecifier Type { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundTypeClause;
    }
}