using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal class BoundEmptyStatement : BoundStatement
    {
        public BoundEmptyStatement(SyntaxToken syntax)
        {
            Syntax = syntax;
        }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundEmptyStatement;
    }
}