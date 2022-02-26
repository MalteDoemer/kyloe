namespace Kyloe.Semantics
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract BoundExpressionResult Result { get; }
    }
}