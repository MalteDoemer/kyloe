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

        public override TypeSpecifier Type => VariableSymbol.Type;

        public override bool HasKnownValue => false;

        public override ValueCategory ValueCategory => VariableSymbol.ValueCategory;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredVariableAccessExpression;
    }
}
