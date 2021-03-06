using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredAssignment : LoweredExpression
    {
        public LoweredAssignment(TypeSystem typeSystem, LoweredExpression leftExpression, AssignmentOperation operation, LoweredExpression rightExpression, MethodType? method)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            Method = method;
            Type = typeSystem.Void;
        }

        public LoweredExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public LoweredExpression RightExpression { get; }
        public MethodType? Method { get; }
        public override TypeInfo Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredAssignment;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return LeftExpression;
            yield return RightExpression;
        }
    }
}
