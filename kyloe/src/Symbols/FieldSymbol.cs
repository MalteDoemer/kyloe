namespace Kyloe.Symbols
{
    internal sealed class FieldSymbol : Symbol
    {
        public FieldSymbol(string name, TypeSpecifier type, bool isReadonly, bool isStatic, AccessModifiers accessModifiers)
        {
            Name = name;
            Type = type;
            IsReadonly = isReadonly;
            IsStatic = isStatic;
            AccessModifiers = accessModifiers;
        }

        public bool IsReadonly { get; }
        public bool IsStatic { get; }
        public AccessModifiers AccessModifiers { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.FieldSymbol;

        public override TypeSpecifier Type { get; }
    }
}