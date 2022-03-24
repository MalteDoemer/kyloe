using System;

namespace Kyloe.Symbols
{
    internal abstract class TypeSpecifier : IEquatable<TypeSpecifier>
    {
        public abstract TypeKind Kind { get; }

        public abstract IReadOnlySymbolScope? ReadOnlyScope { get; }

        public abstract bool Equals(TypeSpecifier? other);

        public abstract string FullName();
    }
}