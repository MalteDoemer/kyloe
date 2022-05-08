using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class ErrorSymbol : Symbol
    {
        public ErrorSymbol(TypeSystem system)
        {
            Type = system.Error;
        }

        public override string Name => string.Empty;

        public override SymbolKind Kind => SymbolKind.ErrorSymbol;

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}