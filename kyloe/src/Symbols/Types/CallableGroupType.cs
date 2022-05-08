using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal class CallableGroupType : TypeInfo
    {
        public CallableGroupType(string name, TypeInfo? parentType)
        {
            Name = name;
            ParentType = parentType;
            Callables = new List<CallableType>();
        }

        public string Name { get; }
        public TypeInfo? ParentType { get; }
        public List<CallableType> Callables { get; }

        public override TypeKind Kind => TypeKind.CallableGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override string FullName() => ParentType is not null ? ParentType.FullName() + "." + Name : Name;
    }
}