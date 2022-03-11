using Kyloe.Symbols;

namespace Kyloe.Semantics 
{
    internal sealed class BoundTypeNameExpression : BoundExpression
    {
        public BoundTypeNameExpression(ITypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
        }

        public ITypeSymbol TypeSymbol { get; }

        public override ISymbol ResultSymbol => TypeSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundTypeNameExpression;
    }
}