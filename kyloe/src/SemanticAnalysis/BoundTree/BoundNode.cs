using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    public abstract class BoundNode
    {
        public abstract SyntaxToken Syntax { get; }
        public abstract BoundNodeKind Kind { get; }
    }
}