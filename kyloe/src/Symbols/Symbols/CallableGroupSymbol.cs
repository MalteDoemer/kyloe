using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal class CallableGroupSymbol : Symbol 
    {
        public CallableGroupSymbol(CallableGroupType group)
        {
            Group = group;
        }

        public CallableGroupType Group { get; }
        
        public override string Name => Group.Name;

        public override SymbolKind Kind => SymbolKind.CallableGroupSymbol;

        public override TypeInfo Type => Group;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}