using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidCallExpression : BoundExpression
    {
        public BoundInvalidCallExpression(TypeSystem typeSystem, BoundExpression expression)
        {
            Expression = expression;
            ResultType = typeSystem.Error;
        }

        public BoundExpression Expression { get; }

        public override TypeSpecifier ResultType { get;  }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidCallExpression;
    }
}