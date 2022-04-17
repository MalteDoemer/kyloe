using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundSymbolExpression : BoundExpression
    {
        public BoundSymbolExpression(Symbol symbol)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get; }

        public override TypeSpecifier ResultType => Symbol.Type;

        public override ValueCategory ValueCategory => Symbol.ValueCategory;

        public override BoundNodeKind Kind => BoundNodeKind.BoundSymbolExpression;
    }
}