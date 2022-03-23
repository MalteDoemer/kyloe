namespace Kyloe.Symbols
{
    internal sealed class TypeNameSymbol : Symbol
    {
        public TypeNameSymbol(ClassType classType)
        {
            Name = classType.Name;
            Type = classType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.TypeNameSymbol;

        public override TypeSpecifier Type { get; }
    }
}