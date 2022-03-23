using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal enum ValueCategory
    {
        ModifiableValue,
        ReadableValue,
        None,
    }

    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSpecifier ResultType { get; }

        // public abstract IReadOnlySymbolScope? SymbolScope { get; }

        public abstract ValueCategory ValueCategory { get; }

        public bool IsValue => ValueCategory != ValueCategory.None;

        public bool IsModifiableValue => ValueCategory == ValueCategory.ModifiableValue;
    }
}