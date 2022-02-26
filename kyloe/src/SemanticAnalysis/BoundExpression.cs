namespace Kyloe.Semantics
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract BoundResultType Result { get; }
    }
}