using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionType : TypeSpecifier
    {
        public FunctionType(string name, TypeSpecifier? parent, bool isStatic, TypeSpecifier returnType)
        {
            Name = name;
            Parent = parent;
            IsStatic = isStatic;
            ReturnType = returnType;
            Parameters = new List<ParameterSymbol>();
        }

        public string Name { get; }
        public TypeSpecifier? Parent { get; }

        public bool IsStatic { get; }
        public TypeSpecifier ReturnType { get; }
        public List<ParameterSymbol> Parameters { get; }

        public override TypeKind Kind => TypeKind.FunctionType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();
            
            builder.Append("func ");
            if (Parent is not null)
                builder.Append(Parent.FullName()).Append('.');
            builder.Append(Name);
            builder.Append('(');
            builder.AppendJoin(',', Parameters.Select(param => param.Type.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }
}