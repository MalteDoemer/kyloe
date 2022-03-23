namespace Kyloe.Symbols
{
    internal abstract class Symbol
    {
        public abstract string Name { get; }
        public abstract SymbolKind Kind { get; }
        public abstract TypeSpecifier Type { get; }
    }
}