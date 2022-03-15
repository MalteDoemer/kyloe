using System;

namespace Kyloe.Symbols
{
    public interface ISymbol : IEquatable<ISymbol?>
    {
        string Name { get; }
        SymbolKind Kind { get; }
    }
}