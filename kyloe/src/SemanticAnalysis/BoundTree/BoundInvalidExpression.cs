using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidExpression : BoundExpression
    {
        private readonly ITypeSymbol errorSymbol;

        public BoundInvalidExpression(TypeSystem typeSystem)
        {
            errorSymbol = typeSystem.Error;
        }

        public override ISymbol ResultSymbol => errorSymbol;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidExpression;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}