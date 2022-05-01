using System.Collections.Generic;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupType : TypeSpecifier
    {
        public FunctionGroupType(string name, TypeSpecifier? parentType)
        {
            Name = name;
            Functions = new List<FunctionType>();
        }

        public string Name { get; }
        public TypeSpecifier? ParentType { get; }
        public List<FunctionType> Functions { get; }

        public override TypeKind Kind => TypeKind.FunctionGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => ParentType is not null ? ParentType.FullName() + "." + Name : Name;
    }
}