using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ErrorTypeSymbol : IErrorTypeSymbol
        {
            public string Name => "<error>";

            public SymbolKind Kind => SymbolKind.ErrorTypeSymbol;

            public IEnumerable<IMethodSymbol> Methods => ImmutableArray<IMethodSymbol>.Empty;

            public IEnumerable<IOperationSymbol> Operations => ImmutableArray<IOperationSymbol>.Empty;

            public IEnumerable<IPropertySymbol> Properties => ImmutableArray<IPropertySymbol>.Empty;

            public IEnumerable<IFieldSymbol> Fields => ImmutableArray<IFieldSymbol>.Empty;

            public AccessModifiers AccessModifiers => AccessModifiers.Public;

            public IEnumerable<IMemberSymbol> Members => ImmutableArray<IMemberSymbol>.Empty;

            public bool Equals(ISymbol? other) => other is IErrorTypeSymbol;

            public IEnumerable<IMemberSymbol> LookupMembers(string name) => ImmutableArray<IMemberSymbol>.Empty;

        }
    }
}