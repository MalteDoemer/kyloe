using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class FunctionGroupSymbol : Symbol
    {
        public FunctionGroupSymbol(FunctionGroupType groupType)
        {
            Name = groupType.Name;
            Type = groupType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.FunctionGroupSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}