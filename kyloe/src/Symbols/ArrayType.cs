using System;

namespace Kyloe.Symbols
{
    internal sealed class ArrayType : TypeSpecifier
    {
        public ArrayType(TypeSpecifier elementType)
        {
            ElementType = elementType;
            Scope = new SymbolScope();
        }

        public TypeSpecifier ElementType { get; }

        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.ArrayType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override string FullName() => ElementType.FullName() + "[]";
    }
}