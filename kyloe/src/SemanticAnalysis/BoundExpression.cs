using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal enum ValueCategory
    {
        LValue,
        RValue,
        None,
    }

    internal abstract class BoundExpression : BoundNode
    {
        public abstract ISymbol ResultSymbol { get; }

        public abstract ValueCategory ValueCategory { get; }

        public bool IsValue => ValueCategory != ValueCategory.None;

        public bool IsLValue => ValueCategory == ValueCategory.LValue;
    }
}