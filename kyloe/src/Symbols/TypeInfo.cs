using System;

namespace Kyloe.Symbols
{
    internal abstract class TypeInfo
    {
        public abstract TypeKind Kind { get; }

        public abstract IReadOnlySymbolScope? ReadOnlyScope { get; }

        public abstract string FullName();

        public override string ToString() => FullName();
    }
}