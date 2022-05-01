using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundParameterDeclaration 
    {
        public BoundParameterDeclaration(ParameterSymbol symbol)
        {
            Symbol = symbol;
        }

        public ParameterSymbol Symbol { get; }

        public TypeSpecifier Type => Symbol.Type;
    }
}