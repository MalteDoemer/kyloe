using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundAssignmentExpression : BoundExpression 
    {
        public BoundAssignmentExpression(TypeSystem typeSystem, BoundExpression leftExpression, AssignmentOperation operation, BoundExpression rightExpression, MethodType? method, SyntaxToken syntax)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = typeSystem.Void;
            Method = method;
            Syntax = syntax;
        }

        public BoundExpression LeftExpression { get; }
        public AssignmentOperation Operation { get; }
        public BoundExpression RightExpression { get; }
        public MethodType? Method { get; }
        public override TypeInfo ResultType { get; }

        public override SyntaxToken Syntax { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeKind Kind => BoundNodeKind.BoundAssignmentExpression;

    }
}