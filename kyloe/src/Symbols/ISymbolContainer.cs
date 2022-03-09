using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface ISymbolContainer
    {
        IEnumerable<ISymbol> Members { get; }
        IEnumerable<ISymbol> LookupMembers(string name);
    }
}