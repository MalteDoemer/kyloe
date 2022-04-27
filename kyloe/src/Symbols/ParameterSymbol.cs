using Kyloe.Semantics;
using Kyloe.Utility;

namespace Kyloe.Symbols
{
    internal sealed class ParameterSymbol : Symbol
    {
        public ParameterSymbol(string name, TypeSpecifier type, SourceLocation? loaction = null)
        {
            Name = name;
            Type = type;
            Loaction = loaction;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.ParameterSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public SourceLocation? Loaction { get; }
    }
}