namespace Kyloe.Lowering
{
    internal sealed class LoweredIfStatement : LoweredStatement
    {
        public LoweredIfStatement(LoweredExpression condition, LoweredStatement body, LoweredStatement elseStatment)
        {
            Condition = condition;
            Body = body;
            ElseStatment = elseStatment;
        }

        public LoweredExpression Condition { get; }
        public LoweredStatement Body { get; }
        public LoweredStatement ElseStatment { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredIfStatement;
    }
}
