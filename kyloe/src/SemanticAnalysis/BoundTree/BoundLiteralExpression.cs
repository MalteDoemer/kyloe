using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(ITypeSymbol result, object value)
        {
            ResultType = result;
            Value = value;
        }

        public object Value { get; }
        public override ITypeSymbol ResultType { get; }

        public override BoundNodeType Type => BoundNodeType.BoundLiteralExpression;

        public override ValueCategory ValueCategory => ValueCategory.RValue;
    }
}