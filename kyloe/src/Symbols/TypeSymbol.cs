using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class TypeSymbol : ITypeSymbol
        {
            private readonly Dictionary<string, List<MethodSymbol>> methods;

            public TypeSymbol(string name)
            {
                Name = name;
                methods = new Dictionary<string, List<MethodSymbol>>();
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.TypeSymbol;

            public IEnumerable<IMethodSymbol> Methods => methods.Values.SelectMany(list => list);

            public IEnumerable<ISymbol> Members => Methods;

            public bool IsErrorType => false;

            public IEnumerable<ISymbol> LookupMembers(string name)
            {
                if (methods.TryGetValue(name, out var list))
                    foreach (var method in list)
                        yield return method;
            }

            public TypeSymbol AddMethod(MethodSymbol method)
            {
                if (methods.TryGetValue(method.Name, out var list))
                {
                    list.Add(method);
                }
                else
                {
                    methods[method.Name] = new List<MethodSymbol>();
                    methods[method.Name].Add(method);
                }

                return this;
            }

            public override string ToString() => Name;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);
        }
    }
}