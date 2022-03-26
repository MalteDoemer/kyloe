using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class GlobalVariableSymbol : Symbol
    {
        public GlobalVariableSymbol(string name, TypeSpecifier type, bool isConst)
        {
            Name = name;
            Type = type;
            IsConst = isConst;
        }

        public bool IsConst { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.GlobalVariableSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }


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

        public override ValueCategory ValueCategory => IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }
}