using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundInvalidExpression : BoundExpression
    {
        private readonly IErrorSymbol errorSymbol;

        public BoundInvalidExpression(TypeSystem typeSystem)
        {
            errorSymbol = typeSystem.Error;
        }

        public override ISymbol ResultSymbol => errorSymbol;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidExpression;

        public override bool IsValue => true; // doesn't matter here
    }
}