namespace Kyloe.Lowering
{
    internal sealed class LoweredExpressionStatement : LoweredStatement 
    {
        public LoweredExpressionStatement(LoweredExpression expression)
        {
            Expression = expression;
        }

        public LoweredExpression Expression { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredExpressionStatement;
    }
}
