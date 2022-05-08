using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupSymbol : Symbol
    {
        public FunctionGroupSymbol(FunctionGroupType groupType)
        {
            Name = groupType.Name;
            Group = groupType;
        }

        public override string Name { get; }
        public FunctionGroupType Group { get; }

        public override SymbolKind Kind => SymbolKind.FunctionGroupSymbol;

        public override TypeInfo Type => Group;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}