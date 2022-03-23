namespace Kyloe.Symbols
{
    internal sealed class MethodGroupSymbol : Symbol
    {
        public MethodGroupSymbol(MethodGroupType groupType)
        {
            Name = groupType.Name;
            Type = groupType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.MethodGroupSymbol;

        public override TypeSpecifier Type { get; }
    }
}