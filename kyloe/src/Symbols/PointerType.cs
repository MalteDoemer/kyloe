namespace Kyloe.Symbols
{
    internal sealed class PointerType : TypeSpecifier
    {
        public PointerType(TypeSpecifier elementType)
        {
            ElementType = elementType;
            Scope = new SymbolScope();
        }

        public TypeSpecifier ElementType { get; }

        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.PointerType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

        public override bool Equals(TypeSpecifier? other) => other is PointerType pointer && pointer.ElementType.Equals(ElementType);

        public override string FullName() => ElementType.FullName() + "*";
    }
}