using System.Collections.Immutable;
using Kyloe.Syntax;

namespace Kyloe.Semantics 
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements, SyntaxToken syntax)
        {
            Statements = statements;
            Syntax = syntax;
        }

        public ImmutableArray<BoundStatement> Statements { get; }
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundBlockStatement;
    }
}
