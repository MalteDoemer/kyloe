using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class NamespaceSymbol : INamespaceSymbol
        {
            private readonly Dictionary<string, NamespaceSymbol> namespaces;

            private readonly Dictionary<string, ClassTypeSymbol> types;

            public NamespaceSymbol(string name)
            {
                Name = name;
                types = new Dictionary<string, ClassTypeSymbol>();
                namespaces = new Dictionary<string, NamespaceSymbol>();
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.NamespaceSymbol;

            public IEnumerable<ITypeSymbol> Types => types.Values;

            public IEnumerable<INamespaceSymbol> Namespaces => namespaces.Values;

            public IEnumerable<IMemberSymbol> Members => namespaces.Values.Cast<IMemberSymbol>().Concat(types.Values);

            public AccessModifiers AccessModifiers => AccessModifiers.Public;

            public IEnumerable<IMemberSymbol> LookupMembers(string name)
            {
                if (namespaces.TryGetValue(name, out var @namespace))
                    yield return @namespace;

                if (types.TryGetValue(name, out var type))
                    yield return type;
            }

            public NamespaceSymbol AddNamespace(NamespaceSymbol @namespace)
            {
                namespaces.Add(@namespace.Name, @namespace);
                return this;
            }

            public NamespaceSymbol? GetNamespace(string name)
            {
                return namespaces.GetValueOrDefault(name);
            }

            public NamespaceSymbol GetOrAddNamespace(string name)
            {
                if (namespaces.TryGetValue(name, out var res))
                    return res;

                var @namespace = new NamespaceSymbol(name);
                namespaces.Add(name, @namespace);

                return @namespace;
            }

            public NamespaceSymbol AddClassType(ClassTypeSymbol type)
            {
                types.Add(type.Name, type);
                return this;
            }

            public ClassTypeSymbol? GetClassType(string name)
            {
                return types.GetValueOrDefault(name);
            }

            public ClassTypeSymbol GetOrAddClassType(string name)
            {
                if (types.TryGetValue(name, out var res))
                    return res;

                var type = new ClassTypeSymbol(name);
                types.Add(name, type);

                return type;
            }

            public override string ToString() => Name;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);
        }
    }
}