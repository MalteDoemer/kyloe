using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Lowering
{
    internal sealed class LoweredStatementBlock : LoweredStatement, IEnumerable<LoweredStatement>
    {
        public LoweredStatementBlock(ImmutableArray<LoweredStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<LoweredStatement> Statements { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredStatementBlock;

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
