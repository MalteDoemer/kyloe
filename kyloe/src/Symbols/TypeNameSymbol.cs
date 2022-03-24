using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class TypeNameSymbol : Symbol
    {
        public TypeNameSymbol(BuiltinType builtinType)
        {
            Name = builtinType.Name;
            Type = builtinType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.TypeNameSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.NoValue;
    }
}