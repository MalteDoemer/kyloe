using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundParenthesizedExpression : BoundExpression 
    {
        public BoundParenthesizedExpression(BoundExpression expression, SyntaxToken syntax)
        {
            Expression = expression;
            Syntax = syntax;
        }

        public BoundExpression Expression { get; }
        
        public override SyntaxToken Syntax { get; }

        public override TypeInfo ResultType => Expression.ResultType;

        public override ValueCategory ValueCategory => Expression.ValueCategory;

        public override BoundNodeKind Kind => BoundNodeKind.BoundParenthesizedExpression;
    }
}