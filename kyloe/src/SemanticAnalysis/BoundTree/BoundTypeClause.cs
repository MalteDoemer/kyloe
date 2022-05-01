using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeClause : BoundNode
    {
        public BoundTypeClause(TypeSpecifier type, SyntaxToken syntax)
        {
            Type = type;
            Syntax = syntax;
        }

        public TypeSpecifier Type { get; }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundTypeClause;
    }
}