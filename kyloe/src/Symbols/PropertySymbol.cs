using Kyloe.Utility;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class PropertySymbol : IPropertySymbol
        {
            private ITypeSymbol? type;
            private IMethodSymbol? getterMethod;
            private IMethodSymbol? setterMethod;
            private bool isStatic;


            public PropertySymbol(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.PropertySymbol;

            public AccessModifiers AccessModifiers => AccessModifiers.Public;

            public ITypeSymbol Type => type ?? throw new NotInitializedException(nameof(Type));

            public IMethodSymbol? GetterMethod => getterMethod;

            public IMethodSymbol? SetterMethod => setterMethod;

            public bool IsStatic => isStatic;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);

            public PropertySymbol SetType(ITypeSymbol type)
            {
                this.type = type;
                return this;
            }

            public PropertySymbol SetSetterMethod(IMethodSymbol setterMethod)
            {
                this.setterMethod = setterMethod;
                return this;
            }

            public PropertySymbol SetGetterMethod(IMethodSymbol getterMethod)
            {
                this.getterMethod = getterMethod;
                return this;
            }

            public PropertySymbol SetStatic(bool value)
            {
                this.isStatic = value;
                return this;
            }
        }
    }
}