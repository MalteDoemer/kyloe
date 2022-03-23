using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class MethodType : TypeSpecifier
    {
        public MethodType(string name, AccessModifiers accessModifiers, TypeSpecifier parent, bool isStatic, TypeSpecifier returnType)
        {
            Name = name;
            AccessModifiers = accessModifiers;
            Parent = parent;
            IsStatic = isStatic;
            ReturnType = returnType;
            ParameterTypes = new List<TypeSpecifier>();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }

        public bool IsStatic { get; }
        public TypeSpecifier ReturnType { get; }
        public List<TypeSpecifier> ParameterTypes { get; }

        public override TypeKind Kind => TypeKind.MethodType;

        public override AccessModifiers AccessModifiers { get; }

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
            builder.Append('(');
            builder.AppendJoin(',', ParameterTypes.Select(param => param.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }
}