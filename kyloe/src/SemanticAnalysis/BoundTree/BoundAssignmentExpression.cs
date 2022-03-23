using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundAssignmentExpression : BoundExpression 
    {
        public BoundAssignmentExpression(TypeSystem typeSystem, BoundExpression leftExpression, AssignmentOperation operation, BoundExpression rightExpression)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = typeSystem.Empty;
        }

        public BoundExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override TypeSpecifier ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundAssignmentExpression;

    }
}