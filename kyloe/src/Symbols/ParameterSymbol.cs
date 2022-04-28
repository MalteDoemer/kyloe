using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Symbols
{
    internal sealed class ParameterSymbol : Symbol
    {
        public ParameterSymbol(string name, TypeSpecifier type, SourceLocation? location = null)
        {
            Name = name;
            Type = type;
            Location = location;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.ParameterSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public SourceLocation? Location { get; }
    }
}