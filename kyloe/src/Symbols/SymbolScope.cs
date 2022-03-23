using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal class SymbolScope : ISymbolScope
    {
        private readonly Dictionary<string, Symbol> symbols;

        public SymbolScope()
        {
            symbols = new Dictionary<string, Symbol>();
        }

        public bool DeclareSymbol(Symbol symbol)
        {
            return symbols.TryAdd(symbol.Name, symbol);
        }

        public Symbol? LookupSymbol(string name)
        {
            return symbols.GetValueOrDefault(name);
        }
    }
}