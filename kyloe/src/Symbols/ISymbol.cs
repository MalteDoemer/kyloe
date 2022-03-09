namespace Kyloe.Symbols
{
    public interface ISymbol
    {
        string Name { get; }
        SymbolKind Kind { get; }
    }
}