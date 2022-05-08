using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class BuiltinFunctionType : CallableType
    {

        public BuiltinFunctionType(CallableGroupType group, TypeInfo returnType)
        {
            Group = group;
            ReturnType = returnType;
            Parameters = new List<ParameterSymbol>();
        }

        public override CallableGroupType Group { get; }

        public override TypeInfo ReturnType { get; }

        public override List<ParameterSymbol> Parameters { get; }

        public override TypeKind Kind => TypeKind.BuiltinFunctionType;

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