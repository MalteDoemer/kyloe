using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Kyloe.Semantics;
using Kyloe.Symbols;

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
            var cond = LogicalNot(condition.Type, condition);
            return new LoweredConditionalGotoStatement(label, cond);
        }

        public static LoweredExpressionStatement ExpressionStatement(LoweredExpression expr)
        {
            return new LoweredExpressionStatement(expr);
        }

        public static LoweredExpression SymbolExpression(Symbol symbol)
        {
            return new LoweredSymbolExpression(symbol);
        }

        public static LoweredExpression LiteralExpression(TypeInfo type, object value)
        {
            return new LoweredLiteralExpression(type, value);
        }

        public static LoweredExpression LogicalNot(TypeInfo type, LoweredExpression expr)
        {
            var name = SemanticInfo.GetFunctionNameFromOperation(BoundOperation.LogicalNot);
            var group = type.ReadOnlyScope?.LookupSymbol(name) as CallableGroupSymbol;

            if (group is not null)
            {
                foreach (var callable in group.Group.Callables)
                {
                    if (callable is MethodType method && callable.Parameters.Count == 1 && callable.ParameterTypes.First().Equals(type))
                        return new LoweredUnaryExpression(expr, BoundOperation.LogicalNot, method);
                }
            }

            throw new System.Exception($"Cannot use LogicalNot() function on type {type.FullName()}");
        }

        public static LoweredExpression Assingment(TypeSystem ts, LoweredExpression left, LoweredExpression right)
        {
            return new LoweredAssignment(ts, left, AssignmentOperation.Assign, right, null);
        }
    }
}