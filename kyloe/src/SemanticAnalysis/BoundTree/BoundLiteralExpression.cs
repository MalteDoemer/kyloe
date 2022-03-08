using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(ISymbol result, object value)
        {
            ResultSymbol = result;
            Value = value;
        }

        public object Value { get; }

        public override ISymbol ResultSymbol { get; }

        public override BoundNodeType Type => BoundNodeType.BoundLiteralExpression;

        public override bool IsValue => true;
    }
}