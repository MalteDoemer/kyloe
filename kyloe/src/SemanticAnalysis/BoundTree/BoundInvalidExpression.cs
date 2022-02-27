namespace Kyloe.Semantics
{
    internal class BoundInvalidExpression : BoundExpression
    {
        public override BoundResultType Result => BoundResultType.ErrorResult;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidExpression;
    }
}