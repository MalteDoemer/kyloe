using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement body, SyntaxToken syntax)
        {
            Condition = condition;
            Body = body;
            Syntax = syntax;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundWhileStatement;
    }
}