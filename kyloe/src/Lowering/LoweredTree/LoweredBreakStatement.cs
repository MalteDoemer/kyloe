using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredBreakStatement : LoweredStatement
    {
        public LoweredBreakStatement(LoweredLabel label)
        {
            Label = label;
        }

        public LoweredLabel Label { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredBreakStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
