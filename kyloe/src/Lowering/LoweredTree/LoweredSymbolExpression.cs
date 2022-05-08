using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredSymbolExpression : LoweredExpression 
    {
        public LoweredSymbolExpression(Symbol symbol)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get; }

        public override TypeInfo Type => Symbol.Type;

        public override ValueCategory ValueCategory => Symbol.ValueCategory;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredSymbolExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
