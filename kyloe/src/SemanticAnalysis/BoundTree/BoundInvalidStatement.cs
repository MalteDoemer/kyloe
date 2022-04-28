namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidStatement : BoundStatement
    {

        public BoundInvalidStatement()
        {
        }

        public override BoundNodeKind Kind => BoundNodeKind.BoundInvalidStatement;
    }
}