using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal class LocalVariableScope
    {
        private readonly Dictionary<string, LocalVariableSymbol> locals;

        public LocalVariableScope()
        {
            locals = new Dictionary<string, LocalVariableSymbol>();
        }

        public bool TryDeclareLocal(string name, ITypeSymbol type)
        {
            return locals.TryAdd(name, new LocalVariableSymbol(name, type));
        }

        public ILocalVariableSymbol? TryGetLocal(string name)
        {
            return locals.GetValueOrDefault(name);
        }

        private sealed class LocalVariableSymbol : ILocalVariableSymbol
        {
            public LocalVariableSymbol(string name, ITypeSymbol type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; }

            public ITypeSymbol Type { get; }

            public SymbolKind Kind => SymbolKind.LocalVariableSymbol;

            public override string ToString() => Name;
        }

    }
}