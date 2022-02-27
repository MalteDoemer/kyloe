namespace Kyloe.Semantics 
{
    internal class BoundEmptyStatement : BoundStatement
    {
        public override BoundNodeType Type => BoundNodeType.BoundEmptyStatement;
    }
}