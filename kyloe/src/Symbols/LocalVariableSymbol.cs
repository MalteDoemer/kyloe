namespace Kyloe.Symbols
{
    internal sealed class LocalVariableSymbol : Symbol
    {
        public LocalVariableSymbol(string name, TypeSpecifier type, bool isConst)
        {
            Name = name;
            Type = type;
            IsConst = isConst;
        }

        public bool IsConst { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.LocalVariableSymbol;

        public override TypeSpecifier Type { get; }
    }
}