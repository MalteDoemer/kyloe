using System.Collections.Immutable;
using Mono.Cecil;

namespace Kyloe.Semantics
{
    public class Namespace
    {
        public Namespace(string name, ImmutableDictionary<string, Namespace> namespaces, ImmutableDictionary<string, TypeReference> types)
        {
            Name = name;
            Namespaces = namespaces;
            Types = types;
        }

        public string Name { get; }
        public ImmutableDictionary<string, Namespace> Namespaces { get; }
        public ImmutableDictionary<string, TypeReference> Types { get; }

        public TypeReference? GetTypeOrNull(string name) => Types.GetValueOrDefault(name);

        public Namespace? GetNamespaceOrNull(string name) => Namespaces.GetValueOrDefault(name);

        public override string? ToString() => Name;
    }
}