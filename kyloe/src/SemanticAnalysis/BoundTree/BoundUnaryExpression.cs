using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        private readonly TypeSystem typeSystem;

        public BoundUnaryExpression(TypeSystem typeSystem, BoundExpression expression, BoundOperation operation, MethodType? method, SyntaxToken syntax)
        {
            this.typeSystem = typeSystem;
            Expression = expression;
            Operation = operation;
            Method = method;
            Syntax = syntax;
        }

        public BoundExpression Expression { get; }
        public BoundOperation Operation { get; }
        public MethodType? Method { get; }

        public override TypeInfo ResultType => Method is null ? typeSystem.Error : Method.ReturnType;

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundUnaryExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }

    internal sealed class BoundConversionExpression : BoundExpression 
    {
        public BoundConversionExpression(BoundExpression expression, MethodType method, SyntaxToken syntax)
        {
            Expression = expression;
            Method = method;
            Syntax = syntax;
        }

        public BoundExpression Expression { get; }
        public MethodType Method { get; }

        public override TypeInfo ResultType => Method.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundConversionExpression;
    }
}