using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredAssignment : LoweredExpression
    {
        public LoweredAssignment(TypeSystem typeSystem, LoweredExpression leftExpression, AssignmentOperation operation, LoweredExpression rightExpression)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            Type = typeSystem.Void;
        }

        public LoweredExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public LoweredExpression RightExpression { get; }
        
        public override TypeSpecifier Type { get; }

        public override bool HasKnownValue => false;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredAssignment;

    }
}
