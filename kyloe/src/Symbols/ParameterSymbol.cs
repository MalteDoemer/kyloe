using Kyloe.Utility;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ParameterSymbol : IParameterSymbol
        {
            private ITypeSymbol? type;

            public ParameterSymbol(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public ITypeSymbol Type => type ?? throw new NotInitializedException(nameof(type));

            public SymbolKind Kind => SymbolKind.ParameterSymbol;

            public ParameterSymbol SetType(ITypeSymbol type)
            {
                this.type = type;
                return this;
            }

            public override string ToString() => Name;
        }
    }
}