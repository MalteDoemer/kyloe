namespace Kyloe.Symbols
{
    internal interface IReadOnlySymbolScope
    {
        Symbol? LookupSymbol(string name);
    }
}