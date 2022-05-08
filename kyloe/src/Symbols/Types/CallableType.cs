using System.Collections.Generic;
using System.Linq;

namespace Kyloe.Symbols
{
    internal abstract class CallableType : TypeInfo
    {
        public abstract CallableGroupType Group { get; }
        public abstract TypeInfo ReturnType { get; }
        public abstract List<ParameterSymbol> Parameters { get; }

        public string Name => Group.Name;
        public TypeInfo? ParentType => Group.ParentType;
        public IEnumerable<TypeInfo> ParameterTypes => Parameters.Select(p => p.Type);

        public override IReadOnlySymbolScope? ReadOnlyScope => null;
    }
}