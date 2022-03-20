using Kyloe.Utility;
using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class OperationSymbol : IOperationSymbol
        {
            private AccessModifiers accessModifiers;
            private IMethodSymbol? underlyingMethod;
            private bool isBuiltin;

            public OperationSymbol(BoundOperation operation)
            {
                Name = SemanticInfo.GetMethodNameFromOperation(operation);
                Operation = operation;
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.OperationSymbol;

            public AccessModifiers AccessModifiers => accessModifiers;

            public BoundOperation Operation { get; }

            public IMethodSymbol UnderlyingMethod => underlyingMethod ?? throw new NotInitializedException(nameof(UnderlyingMethod));

            public bool IsBuiltin => isBuiltin;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);

            public OperationSymbol SetAccessModifiers(AccessModifiers modifiers)
            {
                this.accessModifiers = modifiers;
                return this;
            }

            public OperationSymbol SetUnderlyingMethod(IMethodSymbol method)
            {
                this.underlyingMethod = method;
                return this;
            }

            public OperationSymbol SetBuiltin(bool value)
            {
                this.isBuiltin = value;
                return this;
            }
        }
    }
}