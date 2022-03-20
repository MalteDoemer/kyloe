using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidExpression : BoundExpression
    {

        public BoundInvalidExpression(TypeSystem typeSystem)
        {
            ResultType = typeSystem.Error;
        }

        public override ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidExpression;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}