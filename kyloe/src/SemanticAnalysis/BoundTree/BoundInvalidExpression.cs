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

        public override BoundNodeKind Kind => BoundNodeKind.BoundInvalidExpression;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}