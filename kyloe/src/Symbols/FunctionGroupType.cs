using System.Collections.Generic;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupType : TypeSpecifier
    {
        public FunctionGroupType(string name, TypeSpecifier parent)
        {
            Name = name;
            Parent = parent;
            Functions = new List<FunctionType>();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }
        public List<FunctionType> Functions { get; }

        public override TypeKind Kind => TypeKind.FunctionGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();
            var parentName = Parent.FullName();

            builder.Append("func ");
            if (parentName != "")
                builder.Append(parentName).Append('.');
            builder.Append(Name);
            builder.Append("(...)");

            return builder.ToString();
        }
    }
}