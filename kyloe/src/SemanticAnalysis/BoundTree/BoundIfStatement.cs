using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement body, BoundStatement elifStatement, SyntaxToken syntax)
        {
            Condition = condition;
            Body = body;
            ElifStatement = elifStatement;
            Syntax = syntax;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
        public BoundStatement ElifStatement { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundIfStatement;
    }
}