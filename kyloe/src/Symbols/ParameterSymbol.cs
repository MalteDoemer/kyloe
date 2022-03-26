using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class ParameterSymbol : Symbol
    {
        public ParameterSymbol(string name, TypeSpecifier type)
        {
            Name = name;
            Type = type;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.ParameterSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}