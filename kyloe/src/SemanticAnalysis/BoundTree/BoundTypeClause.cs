using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeClause 
    {
        public BoundTypeClause(BoundExpression expression, TypeSpecifier type)
        {
            Expression = expression;
            Type = type;
        }

        public BoundExpression Expression { get; }
        public TypeSpecifier Type { get; }
    }
}