using System.Collections.Generic;

namespace Kyloe.Lowering
{
    internal sealed class LoweredReturnStatement : LoweredStatement 
    {
        public LoweredReturnStatement(LoweredExpression? expression)
        {
            Expression = expression;
        }

        public LoweredExpression? Expression { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredReturnStatement;

        public override IEnumerable<LoweredNode> Children()
        {
            if (Expression is not null)
                yield return Expression;
        }
    }
}
