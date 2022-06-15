using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        private readonly TypeSystem typeSystem;

        public BoundBinaryExpression(TypeSystem typeSystem, BoundExpression leftExpression, BoundOperation operation, BoundExpression rightExpression, MethodType? method, SyntaxToken syntax)
        {
            this.typeSystem = typeSystem;
            LeftExpression = leftExpression;
            Operation = operation;
            RightExpression = rightExpression;
            Method = method;
            Syntax = syntax;
        }

        public BoundExpression LeftExpression { get; }
        public BoundOperation Operation { get; }
        public BoundExpression RightExpression { get; }
        public MethodType? Method { get; }
        
        public override TypeInfo ResultType => Method is null ? typeSystem.Error : Method.ReturnType;
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundBinaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}