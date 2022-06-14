using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Kyloe.Symbols
{
    internal sealed class BuiltinFunctionType : CallableType
    {

        public BuiltinFunctionType(CallableGroupType group, TypeInfo returnType, Mono.Cecil.MethodReference methodReference)
        {
            Group = group;
            ReturnType = returnType;
            Parameters = new List<ParameterSymbol>();
            MethodReference = methodReference;
        }

        public override CallableGroupType Group { get; }

        public override TypeInfo ReturnType { get; }
        public override List<ParameterSymbol> Parameters { get; }
        
        public MethodReference MethodReference { get; }

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