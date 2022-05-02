using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredGotoStatement : LoweredStatement
    {
        public LoweredGotoStatement(LoweredLabel label)
        {
            Label = label;
        }

        public LoweredLabel Label { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredGotoStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}