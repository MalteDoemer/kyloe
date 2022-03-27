using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class MethodType : TypeSpecifier
    {
        public MethodType(string name, MethodGroupType group, TypeSpecifier returnType, bool isStatic)
        {
            Name = name;
            Group = group;
            ReturnType = returnType;
            IsStatic = isStatic;
            Parameters = new List<ParameterSymbol>();
        }

        public string Name { get; }
        public MethodGroupType Group { get; }
        public TypeSpecifier ReturnType { get; }
        public bool IsStatic { get; }
        public List<ParameterSymbol> Parameters { get; }


        public override TypeKind Kind => TypeKind.MethodType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();

            builder.Append("func ");
            builder.Append(Group.Parent.FullName());
            builder.Append(".");
            builder.Append(Name);
            builder.Append('(');
            builder.AppendJoin(',', Parameters.Select(param => param.Type.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }
}