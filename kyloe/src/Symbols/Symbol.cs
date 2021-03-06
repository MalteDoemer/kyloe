using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal abstract class Symbol
    {
        public abstract string Name { get; }
        public abstract SymbolKind Kind { get; }
        public abstract TypeInfo Type { get; }
        public abstract ValueCategory ValueCategory { get; }
    }
}