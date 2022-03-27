using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal sealed class MethodGroupType : TypeSpecifier
    {
        public MethodGroupType(string name, TypeSpecifier parent)
        {
            Name = name;
            Parent = parent;
            Methods = new List<MethodType>();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }
        public List<MethodType> Methods { get; }

        public override TypeKind Kind => TypeKind.MethodGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => Parent.FullName() + "." + Name;
    }
}