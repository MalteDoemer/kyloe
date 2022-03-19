using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface ITypeSymbol : ISymbol, ISymbolContainer
    {
        IEnumerable<IMethodSymbol> Methods { get; }
    }
}