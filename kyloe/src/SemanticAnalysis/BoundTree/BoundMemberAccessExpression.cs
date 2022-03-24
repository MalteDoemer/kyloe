using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundMemberAccessExpression : BoundExpression
    {
        public BoundMemberAccessExpression(BoundExpression expression, Symbol symbol)
        {
            Expression = expression;
            Symbol = symbol;
        }

        public BoundExpression Expression { get; }
        public Symbol Symbol { get; }

        public override TypeSpecifier ResultType => Symbol.Type;

        public override ValueCategory ValueCategory => Symbol.ValueCategory;

        public override BoundNodeType Type => BoundNodeType.BoundMemberAccessExpression;
    }
}