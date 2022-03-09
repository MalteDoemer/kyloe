namespace Kyloe.Symbols
{
    internal partial class LocalVariableScope
    {
        private sealed class LocalVariableSymbol : ILocalVariableSymbol
        {
            public LocalVariableSymbol(string name, ITypeSymbol type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; }

            public ITypeSymbol Type { get; }

            public SymbolKind Kind => SymbolKind.LocalVariableSymbol;

            public override string ToString() => Name;
        }
    }
}