using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ErrorTypeSymbol : TypeSymbol, IErrorTypeSymbol
        {
            public override string Name => "<error>";

            public override SymbolKind Kind => SymbolKind.ErrorTypeSymbol;

            public override AccessModifiers AccessModifiers => throw new System.NotImplementedException();

            public override IEnumerable<IMethodSymbol> Methods => ImmutableArray<IMethodSymbol>.Empty;

            public override IEnumerable<IFieldSymbol> Fields => ImmutableArray<IFieldSymbol>.Empty;

            public override IEnumerable<ISymbol> Members => ImmutableArray<ISymbol>.Empty;

            public override IEnumerable<IClassTypeSymbol> NestedClasses => ImmutableArray<IClassTypeSymbol>.Empty;

            public override bool Equals(ISymbol? other) => other is IErrorTypeSymbol;

            public override IEnumerable<ISymbol> LookupMembers(string name) => ImmutableArray<ISymbol>.Empty;

            public override TypeSymbol AddMethod(MethodSymbol method)
            {
                throw new NotSupportedException();
            }

            public override TypeSymbol AddNestedClass(ClassTypeSymbol nestedClass)
            {
                throw new NotSupportedException();
            }

            public override TypeSymbol SetAccessModifiers(AccessModifiers modifiers)
            {
                throw new NotSupportedException();
            }
        }
    }
}