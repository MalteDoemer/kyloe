namespace Kyloe.Symbols
{
    internal sealed class ByRefType : TypeSpecifier
    {
        public ByRefType(TypeSpecifier elementType)
        {
            ElementType = elementType;
        }

        public TypeSpecifier ElementType { get; }

        public override IReadOnlySymbolScope? ReadOnlyScope => ElementType.ReadOnlyScope;

        public override TypeKind Kind => TypeKind.ByRefType;

        public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

        public override bool Equals(TypeSpecifier? other) => other is ByRefType byref && byref.ElementType.Equals(ElementType);

        public override string FullName() => ElementType.FullName() + "&";
    }
}