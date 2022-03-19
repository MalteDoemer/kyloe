using System.Collections.Generic;
using System.Text;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private abstract class TypeSymbol : ITypeSymbol
        {
            public abstract string Name { get; }
            public abstract SymbolKind Kind { get; }
            public abstract AccessModifiers AccessModifiers { get; }
            public abstract IEnumerable<IMethodSymbol> Methods { get; }
            public abstract IEnumerable<IFieldSymbol> Fields { get; }
            public abstract IEnumerable<IClassTypeSymbol> NestedClasses { get; }
            public abstract IEnumerable<ISymbol> Members { get; }

            public abstract bool Equals(ISymbol? other);
            public abstract IEnumerable<ISymbol> LookupMembers(string name);

            /// <Summary>
            /// Sets the access modifiers and returns this.
            /// </Summary>
            /// /// <Exceptions>
            /// System.NotSupportedException():
            ///     If the type does not support adding methods.
            /// </Exceptions>
            public abstract TypeSymbol SetAccessModifiers(AccessModifiers modifiers);

            /// <Summary>
            /// Adds the method and returns this.
            /// </Summary>
            /// <Exceptions>
            /// System.NotSupportedException():
            ///     If the type does not support adding methods.
            /// </Exceptions>
            public abstract TypeSymbol AddMethod(MethodSymbol method);

            /// <Summary>
            /// Adds the nested class and returns this.
            /// </Summary>
            /// System.NotSupportedException():
            ///     If the type does not support adding nested classes.
            /// </Exceptions>
            public abstract TypeSymbol AddNestedClass(ClassTypeSymbol nestedClass);

        }
    }
}