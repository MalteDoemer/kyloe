using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(BoundStatement declarationStatement, BoundExpression condition, BoundExpression increment, BoundStatement body, SyntaxToken syntax)
        {
            DeclarationStatement = declarationStatement;
            Condition = condition;
            Increment = increment;
            Body = body;
            Syntax = syntax;
        }

        public BoundStatement DeclarationStatement { get; }
        public BoundExpression Condition { get; }
        public BoundExpression Increment { get; }
        public BoundStatement Body { get; }

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundForStatement;
    }
}