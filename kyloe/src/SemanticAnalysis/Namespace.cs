using System.Collections.Immutable;
using Mono.Cecil;

namespace Kyloe.Semantics
{
    public class Namespace
    {
        public Namespace(ImmutableDictionary<string, Namespace> namespaces, ImmutableDictionary<string, TypeReference> types)
        {
            Namespaces = namespaces;
            Types = types;
        }

        public ImmutableDictionary<string, Namespace> Namespaces { get; }
        public ImmutableDictionary<string, TypeReference> Types { get; }

        public TypeReference? GetTypeOrNull(string name) => Types.GetValueOrDefault(name);

        public Namespace? GetNamespaceOrNull(string name) => Namespaces.GetValueOrDefault(name);
    }
}