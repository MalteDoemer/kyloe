using System.Collections.Immutable;

namespace Kyloe.Semantics 
{
    internal class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<BoundStatement> Statements { get; }

        public override BoundNodeType Type => BoundNodeType.BoundBlockStatement;
    }
}
