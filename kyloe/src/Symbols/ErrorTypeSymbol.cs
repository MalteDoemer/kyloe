using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ErrorTypeSymbol : IErrorTypeSymbol
        {
            public string Name => "<error>";

            public bool IsErrorType => true;

            public SymbolKind Kind => SymbolKind.ErrorSymbol;

            public IEnumerable<IMethodSymbol> Methods => ImmutableArray<IMethodSymbol>.Empty;

            public IEnumerable<ISymbol> Members => ImmutableArray<ISymbol>.Empty;

            public bool Equals(ISymbol? other) => other is IErrorTypeSymbol;

            public IEnumerable<ISymbol> LookupMembers(string name) => ImmutableArray<ISymbol>.Empty;
        }
    }
}