using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression leftExpression, BoundOperation operation, BoundExpression rightExpression, TypeInfo result, SyntaxToken syntax)
        {
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            ResultType = result;
            Syntax = syntax;
        }

        public BoundExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public BoundExpression RightExpression { get; }

        public override TypeInfo ResultType { get; }
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundBinaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}