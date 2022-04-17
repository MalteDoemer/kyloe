namespace Kyloe.Semantics
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement body, BoundStatement? elseBody = null)
        {
            Condition = condition;
            Body = body;
            ElseBody = elseBody;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
        public BoundStatement? ElseBody { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundIfStatement;
    }

    internal class BoundEmptyStatement : BoundStatement
    {
        public override BoundNodeKind Kind => BoundNodeKind.BoundEmptyStatement;
    }
}