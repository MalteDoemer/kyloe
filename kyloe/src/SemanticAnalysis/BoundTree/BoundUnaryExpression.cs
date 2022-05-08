using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression expression, BoundOperation operation, TypeInfo result, SyntaxToken syntax)
        {
            Expression = expression;
            Operation = operation;
            ResultType = result;
            Syntax = syntax;
        }

        public BoundExpression Expression { get; }
        public BoundOperation Operation { get; }
        
        public override TypeInfo ResultType { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundUnaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}