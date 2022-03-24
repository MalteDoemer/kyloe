using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidExpression : BoundExpression
    {

        public BoundInvalidExpression(TypeSystem typeSystem)
        {
            ResultType = typeSystem.Error;
        }

        public override TypeSpecifier ResultType { get; }

        public override BoundNodeType Type => BoundNodeType.BoundInvalidExpression;

        public override ValueCategory ValueCategory => ValueCategory.NoValue;
    }
}