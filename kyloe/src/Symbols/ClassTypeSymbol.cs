namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ClassTypeSymbol : TypeSymbolBase, IClassTypeSymbol
        {
            public ClassTypeSymbol(string name) : base(name) { }

            public SymbolKind Kind => SymbolKind.ClassTypeSymbol;

            public override string ToString() => Name;
        }
    }
}