using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeNameExpression : BoundExpression
    {
        public BoundTypeNameExpression(ITypeSymbol typeSymbol)
        {
            ResultType = typeSymbol;
        }

        public override ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundTypeNameExpression;
    }
}