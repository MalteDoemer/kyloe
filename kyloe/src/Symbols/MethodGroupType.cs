using System.Collections.Generic;
using System.Text;

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