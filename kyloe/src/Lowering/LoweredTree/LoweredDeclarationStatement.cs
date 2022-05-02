using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredDeclarationStatement : LoweredStatement
    {
        public LoweredDeclarationStatement(Symbol symbol, LoweredExpression? initializer)
        {
            Symbol = symbol;
            Initializer = initializer;
        }

        public Symbol Symbol { get; }
        public LoweredExpression? Initializer { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredDeclarationStatement;
    }
}
