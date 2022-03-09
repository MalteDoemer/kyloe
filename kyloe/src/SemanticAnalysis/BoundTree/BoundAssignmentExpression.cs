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
            ResultSymbol = typeSystem.Empty;
        }

        public BoundExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override ISymbol ResultSymbol { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundAssignmentExpression;
    }
}