namespace Kyloe.Semantics
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement body, BoundStatement elifStatement)
        {
            Condition = condition;
            Body = body;
            ElifStatement = elifStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
        public BoundStatement ElifStatement { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundIfStatement;
    }
}