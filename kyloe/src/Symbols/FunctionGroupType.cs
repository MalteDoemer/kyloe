using System.Collections.Generic;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupType : TypeSpecifier
    {
        public FunctionGroupType(string name)
        {
            Name = name;
            Functions = new List<FunctionType>();
        }

        public string Name { get; }
        public List<FunctionType> Functions { get; }

        public override TypeKind Kind => TypeKind.FunctionGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => Name;
    }
}