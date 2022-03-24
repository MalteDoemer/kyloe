namespace Kyloe.Symbols
{
    internal sealed class BuiltinType : TypeSpecifier
    {
        public BuiltinType(string name)
        {
            Name = name;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.BuiltinType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => Name;
    }
}