using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundBreakStatement : BoundStatement
    {
        public BoundBreakStatement(SyntaxToken syntax)
        {
            Syntax = syntax;
        }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundBreakStatement;
    }
}