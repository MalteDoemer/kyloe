using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeClause 
    {
        public BoundTypeClause(BoundExpression expression, ITypeSymbol typeSymbol)
        {
            Expression = expression;
            TypeSymbol = typeSymbol;
        }

        public BoundExpression Expression { get; }
        public ITypeSymbol TypeSymbol { get; }
    }
}