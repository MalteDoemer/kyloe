using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ArrayTypeSymbol : TypeSymbol, IArrayTypeSymbol
        {
            private readonly Dictionary<string, List<MethodSymbol>> methods;

            public ArrayTypeSymbol(ITypeSymbol elementType)
            {
                ElementType = elementType;
                Name = elementType.Name + "[]";

                methods = new Dictionary<string, List<MethodSymbol>>();

                // TODO: initialize methods
            }

            public ITypeSymbol ElementType { get; }

            public override string Name { get; }

            public override SymbolKind Kind => SymbolKind.ArrayTypeSymbol;

            public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

            public override IEnumerable<IMethodSymbol> Methods => methods.Values.SelectMany(list => list);

            public override IEnumerable<IFieldSymbol> Fields => ImmutableArray<IFieldSymbol>.Empty;

            public override IEnumerable<IClassTypeSymbol> NestedClasses => ImmutableArray<IClassTypeSymbol>.Empty;

            public override IEnumerable<ISymbol> Members => Methods.Cast<ISymbol>().Concat(Fields);

            public override bool Equals(ISymbol? other)
            {
                return other is ArrayTypeSymbol array && array.ElementType.Equals(ElementType);
            }

            public override IEnumerable<ISymbol> LookupMembers(string name)
            {
                if (methods.TryGetValue(name, out var list))
                    foreach (var method in list)
                        yield return method;
            }

            public override ArrayTypeSymbol AddMethod(MethodSymbol method)
            {
                if (methods.TryGetValue(method.Name, out var list))
                {
                    list.Add(method);
                }
                else
                {
                    var newlist = new List<MethodSymbol>();
                    newlist.Add(method);
                    methods.Add(method.Name, newlist);
                }

                return this;
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

        private sealed class ClassTypeSymbol : TypeSymbol, IClassTypeSymbol
        {
            private AccessModifiers accessModifiers;
            private readonly Dictionary<string, List<MethodSymbol>> methods;
            private readonly Dictionary<string, ClassTypeSymbol> nestedClasses;

            public ClassTypeSymbol(string name)
            {
                Name = name;
                methods = new Dictionary<string, List<MethodSymbol>>();
                nestedClasses = new Dictionary<string, ClassTypeSymbol>();
            }

            public override string Name { get; }
            public override SymbolKind Kind => SymbolKind.ClassTypeSymbol;
            public override AccessModifiers AccessModifiers => accessModifiers;
            public override IEnumerable<IMethodSymbol> Methods => methods.Values.SelectMany(list => list);
            public override IEnumerable<IFieldSymbol> Fields => ImmutableArray<IFieldSymbol>.Empty;
            public override IEnumerable<IClassTypeSymbol> NestedClasses => nestedClasses.Values;

            public override IEnumerable<ISymbol> Members => Methods.Cast<ISymbol>().Concat(Fields).Concat(NestedClasses);

            public override IEnumerable<ISymbol> LookupMembers(string name)
            {
                if (methods.TryGetValue(name, out var list))
                    foreach (var method in list)
                        yield return method;
                if (nestedClasses.TryGetValue(name, out var nested))
                    yield return nested;
            }

            public override ClassTypeSymbol SetAccessModifiers(AccessModifiers modifiers)
            {
                accessModifiers = modifiers;
                return this;
            }

            public override ClassTypeSymbol AddMethod(MethodSymbol method)
            {
                if (methods.TryGetValue(method.Name, out var list))
                {
                    list.Add(method);
                }
                else
                {
                    var newlist = new List<MethodSymbol>();
                    newlist.Add(method);
                    methods.Add(method.Name, newlist);
                }

                return this;
            }

            public override ClassTypeSymbol AddNestedClass(ClassTypeSymbol type)
            {
                nestedClasses.Add(type.Name, type);
                return this;
            }

            public override string ToString() => Name;

            public override bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);
        }
    }
}