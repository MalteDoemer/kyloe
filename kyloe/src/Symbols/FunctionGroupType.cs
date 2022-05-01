using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupType : TypeSpecifier
    {
        public FunctionGroupType(string name, TypeSpecifier? parentType)
        {
            Name = name;
            Functions = new List<FunctionType>();
        }

        public string Name { get; }
        public TypeSpecifier? ParentType { get; }

        public List<FunctionType> Functions { get; }

        public override TypeKind Kind => TypeKind.FunctionGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other)
        {
            if (!(other is FunctionGroupType group))
                return false;

            if (Name != group.Name || !object.Equals(ParentType, group.ParentType) || Functions.Count != group.Functions.Count)
                return false;

            foreach (var (f1, f2) in Functions.Zip(group.Functions))
                if (!f1.Equals(f2))
                    return false;

            return true;
        }

        public override string FullName() => ParentType is not null ? ParentType.FullName() + "." + Name : Name;
    }
}