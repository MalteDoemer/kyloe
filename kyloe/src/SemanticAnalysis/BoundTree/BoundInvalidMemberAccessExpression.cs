using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidMemberAccessExpression : BoundExpression
    {
        public BoundInvalidMemberAccessExpression(TypeSystem typeSystem, BoundExpression expression, string name)
        {
            ResultType = typeSystem.Error;
            Expression = expression;
            Name = name;
        }

        public BoundExpression Expression { get; }
        public string Name { get; }

        public override TypeSpecifier ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.NoValue;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidMemberAccessExpression;
    }
}