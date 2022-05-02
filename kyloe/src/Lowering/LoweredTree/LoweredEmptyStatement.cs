using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredEmptyStatement : LoweredStatement
    {
        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredEmptyStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
