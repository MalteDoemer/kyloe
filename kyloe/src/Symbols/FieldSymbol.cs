using Kyloe.Utility;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class FieldSymbol : IFieldSymbol
        {
            private ITypeSymbol? type;
            private AccessModifiers accessModifiers;
            private bool isReadonly;
            private bool isStatic;

            public FieldSymbol(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.FieldSymbol;

            public ITypeSymbol Type => type ?? throw new NotInitializedException(nameof(Type));

            public AccessModifiers AccessModifiers => accessModifiers;

            public bool IsReadonly => isReadonly;

            public bool IsStatic => isStatic;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);

            public FieldSymbol SetType(ITypeSymbol type)
            {
                this.type = type;
                return this;
            }

            public FieldSymbol SetAccessModifiers(AccessModifiers modifiers)
            {
                this.accessModifiers = modifiers;
                return this;
            }

            public FieldSymbol SetReadonly(bool value)
            {
                this.isReadonly = value;
                return this;
            }

            public FieldSymbol SetStatic(bool value)
            {
                this.isStatic = value;
                return this;
            }
        }
    }
}