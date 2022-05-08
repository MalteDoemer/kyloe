using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupType : TypeInfo
    {
        public FunctionGroupType(string name, ValueTypeInfo? parentType)
        {
            Name = name;
            Functions = new List<FunctionTypeInfo>();
        }

        public string Name { get; }
        public ValueTypeInfo? ParentType { get; }

        public List<FunctionTypeInfo> Functions { get; }

        public override TypeKind Kind => TypeKind.FunctionGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override string FullName() => ParentType is not null ? ParentType.FullName() + "." + Name : Name;
    }
}