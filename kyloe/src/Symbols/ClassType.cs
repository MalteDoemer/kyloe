namespace Kyloe.Symbols
{
    internal sealed class ClassType : TypeSpecifier
    {
        public ClassType(string name, AccessModifiers accessModifiers, TypeSpecifier parent)
        {
            Name = name;
            AccessModifiers = accessModifiers;
            Parent = parent;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.ClassType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers { get; }

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var parentName = Parent.FullName();

            if (parentName == "")
                return Name;

            return parentName + "." + Name;
        }
    }
}