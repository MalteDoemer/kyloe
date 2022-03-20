using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface INamespaceSymbol : IMemberSymbol, IMemberContainer
    {
        IEnumerable<ITypeSymbol> Types { get; }
        IEnumerable<INamespaceSymbol> Namespaces { get; }
    }
}