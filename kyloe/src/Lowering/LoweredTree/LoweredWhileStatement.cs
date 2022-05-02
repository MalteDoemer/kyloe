using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredWhileStatement : LoweredStatement
    {
        public LoweredWhileStatement(LoweredLabel breakLabel, LoweredLabel continueLabel, LoweredExpression condition, LoweredStatement body)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
            Condition = condition;
            Body = body;
        }

        public LoweredLabel BreakLabel { get; }
        public LoweredLabel ContinueLabel { get; }

        public LoweredExpression Condition { get; }
        public LoweredStatement Body { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredWhileStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Condition;
            yield return Body;
        }
    }
}
