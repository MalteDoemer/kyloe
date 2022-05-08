using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionType : CallableType
    {

        public FunctionType(CallableGroupType group, TypeInfo returnType)
        {
            Group = group;
            ReturnType = returnType;
            Parameters = new List<ParameterSymbol>();
        }

        public override CallableGroupType Group { get; }

        public override TypeInfo ReturnType { get; }

        public override List<ParameterSymbol> Parameters { get; }

        public override TypeKind Kind => TypeKind.FunctionType;

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

    internal sealed class MethodType : CallableType
    {
        public MethodType(CallableGroupType group, TypeInfo returnType, bool isStatic = false, bool isOperator = false)
        {
            Debug.Assert(group.ParentType is not null);

            Group = group;
            ReturnType = returnType;
            IsStatic = isStatic;
            IsOperator = isOperator;
            Parameters = new List<ParameterSymbol>();
        }

        public bool IsStatic { get; set; }
        public bool IsOperator { get; set; }

        public override CallableGroupType Group { get; }

        public override TypeInfo ReturnType { get; }

        public override List<ParameterSymbol> Parameters { get; }

        public override TypeKind Kind => TypeKind.MethodType;

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