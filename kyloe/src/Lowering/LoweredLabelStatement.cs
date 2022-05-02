namespace Kyloe.Lowering
{
    internal sealed class LoweredLabelStatement : LoweredStatement 
    {
        public LoweredLabelStatement(LoweredLabel label)
        {
            Label = label;
        }

        public LoweredLabel Label { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredLabelStatement;
    }
}