using System;

namespace Kyloe.Symbols
{
    internal sealed class BuiltinType : ValueTypeInfo
    {
        public BuiltinType(string name)
        {
            Name = name;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.BuiltinType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override string FullName() => Name;
    }
}