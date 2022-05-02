namespace Kyloe.Lowering
{
    internal sealed class LoweredEmptyStatement : LoweredStatement
    {
        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredEmptyStatement;
    }
}
