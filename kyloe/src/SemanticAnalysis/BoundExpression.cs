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
        public abstract ITypeSymbol ResultType { get; }

        public abstract ISymbol ResultSymbol { get; }

        // TODO: make this context specific for things like private set properties or readonly fields in the constructor
        public abstract ValueCategory ValueCategory { get; }

        public bool IsValue => ValueCategory != ValueCategory.None;

        public bool IsModifiableValue => ValueCategory == ValueCategory.ModifiableValue;
    }
}