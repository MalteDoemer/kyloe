namespace Kyloe.Symbols
{

    sealed class PropertySymbol : Symbol
    {
        public PropertySymbol(string name, TypeSpecifier type, bool isStatic, MethodType? getMethod, MethodType? setMethod)
        {
            Name = name;
            Type = type;
            IsStatic = isStatic;
            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        public bool IsStatic { get; }
        public MethodType? GetMethod { get; }
        public MethodType? SetMethod { get; }
        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.PropertySymbol;

        public override TypeSpecifier Type { get; }
    }
}