using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredVariableAccessExpression : LoweredExpression 
    {
        public LoweredVariableAccessExpression(Symbol symbol)
        {
            VariableSymbol = symbol;
        }

        public Symbol VariableSymbol { get; }

        public override TypeInfo Type => VariableSymbol.Type;

        public override ValueCategory ValueCategory => VariableSymbol.ValueCategory;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredVariableAccessExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
