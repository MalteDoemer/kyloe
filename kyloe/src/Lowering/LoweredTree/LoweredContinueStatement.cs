using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredContinueStatement : LoweredStatement
    {
        public LoweredContinueStatement(LoweredLabel label)
        {
            Label = label;
        }

        public LoweredLabel Label { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredContinueStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
