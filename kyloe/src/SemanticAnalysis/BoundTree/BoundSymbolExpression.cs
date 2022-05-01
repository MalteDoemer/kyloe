using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundSymbolExpression : BoundExpression
    {
        public BoundSymbolExpression(Symbol symbol, SyntaxToken syntax)
        {
            Symbol = symbol;
            Syntax = syntax;
        }

        public Symbol Symbol { get; }

        public override SyntaxToken Syntax { get; }

        public override TypeSpecifier ResultType => Symbol.Type;

        public override ValueCategory ValueCategory => Symbol.ValueCategory;

        public override BoundNodeKind Kind => BoundNodeKind.BoundSymbolExpression;
    }
}