namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class PointerTypeSymbol : TypeSymbolBase, IPointerTypeSymbol
        {
            public PointerTypeSymbol(ITypeSymbol underlyingType) : base(underlyingType.Name + "*")
            {
                UnderlyingType = underlyingType;
            }

            public ITypeSymbol UnderlyingType { get; }

            public SymbolKind Kind => SymbolKind.PointerTypeSymbol;
        }
    }
}