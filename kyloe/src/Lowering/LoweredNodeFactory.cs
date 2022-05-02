using System.Collections.Immutable;
using Kyloe.Semantics;

namespace Kyloe.Lowering 
{
    internal static class LoweredNodeFactory 
    {
        public static LoweredBlockStatement Block(params LoweredStatement[] statements) 
        {
            return new LoweredBlockStatement(statements.ToImmutableArray());
        }

        public static LoweredLabelStatement LabelStatement(LoweredLabel label) 
        {
            return new LoweredLabelStatement(label);
        }

        public static LoweredGotoStatement Goto(LoweredLabel label) 
        {
            return new LoweredGotoStatement(label);
        }

        public static LoweredConditionalGotoStatement GotoIf(LoweredLabel label, LoweredExpression condition) 
        {
            return new LoweredConditionalGotoStatement(label, condition);
        }

        public static LoweredConditionalGotoStatement GotoIfNot(LoweredLabel label, LoweredExpression condition) 
        {
            var cond = new LoweredUnaryExpression(condition.Type, condition, Semantics.BoundOperation.LogicalNot);
            return new LoweredConditionalGotoStatement(label, cond);
        }

        public static LoweredExpressionStatement ExpressionStatement(LoweredExpression expr) 
        {
            return new LoweredExpressionStatement(expr);
        }
    }
}