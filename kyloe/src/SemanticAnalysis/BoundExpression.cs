using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract ISymbol ResultSymbol { get; }

        public abstract bool IsValue { get; }
    }
}