namespace Kyloe.Lowering
{
    internal sealed class LoweredConditionalGotoStatement : LoweredStatement
    {
        public LoweredConditionalGotoStatement(LoweredLabel label, LoweredExpression condition)
        {
            Label = label;
            Condition = condition;
        }

        public LoweredLabel Label { get; }
        public LoweredExpression Condition { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredConditionalGotoStatement;
    }
}