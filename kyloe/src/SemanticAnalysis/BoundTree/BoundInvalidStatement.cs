using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidStatement : BoundStatement
    {

        public BoundInvalidStatement(SyntaxToken syntax)
        {
            Syntax = syntax;
        }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundInvalidStatement;
    }
}