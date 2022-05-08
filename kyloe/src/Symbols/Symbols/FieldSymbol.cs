using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class FieldSymbol : Symbol
    {
        public FieldSymbol(string name, TypeInfo type, bool isReadonly, bool isStatic)
        {
            Name = name;
            Type = type;
            IsReadonly = isReadonly;
            IsStatic = isStatic;
        }

        public bool IsReadonly { get; }
        public bool IsStatic { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.FieldSymbol;

        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => IsReadonly ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }
}