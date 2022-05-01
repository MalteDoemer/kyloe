using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundAssignmentExpression : BoundExpression 
    {
        public BoundAssignmentExpression(TypeSystem typeSystem, BoundExpression leftExpression, AssignmentOperation operation, BoundExpression rightExpression, SyntaxToken syntax)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = typeSystem.Void;
            Syntax = syntax;
        }

        public BoundExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public BoundExpression RightExpression { get; }
        public override TypeSpecifier ResultType { get; }

        public override SyntaxToken Syntax { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeKind Kind => BoundNodeKind.BoundAssignmentExpression;

    }
}