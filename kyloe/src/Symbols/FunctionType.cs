using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionType : TypeSpecifier
    {
        public FunctionType(string name, FunctionGroupType group, TypeSpecifier returnType)
        {
            Name = name;
            Group = group;
            ReturnType = returnType;
            Parameters = new List<ParameterSymbol>();
        }

        public string Name { get; }
        public FunctionGroupType Group { get; }
        public TypeSpecifier ReturnType { get; }
        public List<ParameterSymbol> Parameters { get; }

        public override TypeKind Kind => TypeKind.FunctionType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();

            builder.Append("func ");
            builder.Append(Name);
            builder.Append('(');
            builder.AppendJoin(',', Parameters.Select(param => param.Type.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }
}