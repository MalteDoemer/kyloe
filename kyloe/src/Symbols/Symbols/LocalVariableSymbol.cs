using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class LocalVariableSymbol : Symbol
    {
        public LocalVariableSymbol(string name, TypeInfo type, bool isConst)
        {
            Name = name;
            Type = type;
            IsConst = isConst;
        }

        public bool IsConst { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.LocalVariableSymbol;

        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }
}