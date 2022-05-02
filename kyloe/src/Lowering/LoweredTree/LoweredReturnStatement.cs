namespace Kyloe.Lowering
{
    internal sealed class LoweredReturnStatement : LoweredStatement 
    {
        public LoweredReturnStatement(LoweredExpression? expression)
        {
            Expression = expression;
        }

        public LoweredExpression? Expression { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredReturnStatement;
    }
}
