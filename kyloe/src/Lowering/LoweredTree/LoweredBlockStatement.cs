using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Lowering
{
    internal sealed class LoweredBlockStatement : LoweredStatement, IEnumerable<LoweredStatement>
    {
        public LoweredBlockStatement(ImmutableArray<LoweredStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<LoweredStatement> Statements { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredBlockStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            return Statements;
        }

        public IEnumerator<LoweredStatement> GetEnumerator()
        {
            return ((IEnumerable<LoweredStatement>)Statements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Statements).GetEnumerator();
        }
    }
}
