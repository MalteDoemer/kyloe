using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionType : TypeSpecifier
    {
        public FunctionType(FunctionGroupType group, TypeSpecifier returnType, bool isStatic, bool isBuiltin = false)
        {
            Group = group;
            ReturnType = returnType;
            IsStatic = isStatic;
            IsBuiltin = isBuiltin;
            Parameters = new List<ParameterSymbol>();
        }

        public FunctionGroupType Group { get; }
        public TypeSpecifier ReturnType { get; }
        public bool IsStatic { get; }
        public bool IsBuiltin { get; }

        public List<ParameterSymbol> Parameters { get; }

        public string Name => Group.Name;

        public TypeSpecifier? ParentType => Group.ParentType;
        
        public override TypeKind Kind => TypeKind.FunctionType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override string FullName()
        {
            var builder = new StringBuilder();

            builder.Append("func ");
            builder.Append(Group.FullName());
            builder.Append('(');
            builder.AppendJoin(',', Parameters.Select(param => param.Type.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }
}