using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundContinueStatement : BoundStatement
    {
        public BoundContinueStatement(SyntaxToken syntax)
        {
            Syntax = syntax;
        }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundContinueStatement;
    }
}