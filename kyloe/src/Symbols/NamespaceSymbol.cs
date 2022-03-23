namespace Kyloe.Symbols
{
    internal sealed class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(NamespaceType namespaceType)
        {
            Name = namespaceType.Name;
            Type = namespaceType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.NamespaceSymbol;

        public override TypeSpecifier Type { get; }
    }
}