namespace Kyloe.Symbols
{
    internal interface ISymbolScope : IReadOnlySymbolScope
    {
        bool DeclareSymbol(Symbol symbol);
    }
}