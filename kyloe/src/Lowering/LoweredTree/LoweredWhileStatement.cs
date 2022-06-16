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

    internal sealed class LoweredForStatement : LoweredStatement
    {
        public LoweredForStatement(LoweredLabel breakLabel, LoweredLabel continueLabel, LoweredStatement declarationStatement, LoweredExpression condition, LoweredExpression increment, LoweredStatement body)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
            DeclarationStatement = declarationStatement;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public LoweredLabel BreakLabel { get; }
        public LoweredLabel ContinueLabel { get; }
        public LoweredStatement DeclarationStatement { get; }
        public LoweredExpression Condition { get; }
        public LoweredExpression Increment { get; }
        public LoweredStatement Body { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredForStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return DeclarationStatement;
            yield return Condition;
            yield return Increment;
            yield return Body;
        }
    }
}
