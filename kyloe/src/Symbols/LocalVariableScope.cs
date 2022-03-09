using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal partial class LocalVariableScope
    {
        private readonly Dictionary<string, LocalVariableSymbol> locals;

        public LocalVariableScope()
        {
            locals = new Dictionary<string, LocalVariableSymbol>();
        }

        public bool TryDeclareLocal(string name, ITypeSymbol type, bool isConst)
        {
            return locals.TryAdd(name, new LocalVariableSymbol(name, type, isConst));
        }

        public ILocalVariableSymbol? TryGetLocal(string name)
        {
            return locals.GetValueOrDefault(name);
        }
    }
}