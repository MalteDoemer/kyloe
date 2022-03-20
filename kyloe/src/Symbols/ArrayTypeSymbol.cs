namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ArrayTypeSymbol : TypeSymbolBase, IArrayTypeSymbol
        {
            public ArrayTypeSymbol(ITypeSymbol elementType) : base(elementType.Name + "[]")
            {
                ElementType = elementType;
            }

            public ITypeSymbol ElementType { get; }

            public SymbolKind Kind => SymbolKind.ArrayTypeSymbol;
        }
    }
}