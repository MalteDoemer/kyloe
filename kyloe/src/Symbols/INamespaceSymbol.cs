using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface INamespaceSymbol : ISymbol, ISymbolContainer
    {
        IEnumerable<ITypeSymbol> Types { get; }
        IEnumerable<INamespaceSymbol> Namespaces { get; }
    }
}