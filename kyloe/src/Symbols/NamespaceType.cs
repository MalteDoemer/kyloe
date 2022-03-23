namespace Kyloe.Symbols
{
    internal sealed class NamespaceType : TypeSpecifier
    {
        public NamespaceType(string name, NamespaceType? parent)
        {
            Name = name;
            Parent = parent;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public NamespaceType? Parent { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.NamespaceType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers => AccessModifiers.Public;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => (Parent is null ? "" : Parent.FullName() + ".") + Name;
    }
}