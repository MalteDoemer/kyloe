using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Symbols
{
    internal sealed class ParameterSymbol : Symbol
    {
        public ParameterSymbol(string name, int index, TypeInfo type)
        {
            Name = name;
            Index = index;
            Type = type;
        }

        public override string Name { get; }
        
        public int Index { get; }

        public override SymbolKind Kind => SymbolKind.ParameterSymbol;

        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}