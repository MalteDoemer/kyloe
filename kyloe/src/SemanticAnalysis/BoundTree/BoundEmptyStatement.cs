namespace Kyloe.Semantics
{
    internal class BoundEmptyStatement : BoundStatement
    {
        public override BoundNodeKind Kind => BoundNodeKind.BoundEmptyStatement;
    }
}