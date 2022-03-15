namespace Kyloe.Symbols
{
    internal partial class LocalVariableScope
    {
        private sealed class LocalVariableSymbol : ILocalVariableSymbol
        {
            public LocalVariableSymbol(string name, ITypeSymbol type, bool isConst)
            {
                Name = name;
                Type = type;
                IsConst = isConst;
            }

            public string Name { get; }
            public ITypeSymbol Type { get; }
            public bool IsConst { get; }

            public SymbolKind Kind => SymbolKind.LocalVariableSymbol;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);

            public override string ToString() => Name;
        }
    }
}