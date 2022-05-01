using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression, SyntaxToken syntax)
        {
            Expression = expression;
            Syntax = syntax;
        }

        public BoundExpression Expression { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundExpressionStatement;
    }
}