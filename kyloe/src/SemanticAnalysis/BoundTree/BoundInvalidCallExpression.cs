using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionType function, BoundExpression expression, BoundArguments arguments)
        {
            Function = function;
            Expression = expression;
            Arguments = arguments;
        }

        public FunctionType Function { get; }
        public BoundExpression Expression { get; }
        public BoundArguments Arguments { get; }

        public override TypeSpecifier ResultType => Function.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override BoundNodeType Type => BoundNodeType.BoundCallExpression;
    }

    internal sealed class BoundInvalidCallExpression : BoundExpression
    {
        public BoundInvalidCallExpression(TypeSystem typeSystem, BoundExpression expression)
        {
            Expression = expression;
            ResultType = typeSystem.Error;
        }

        public BoundExpression Expression { get; }

        public override TypeSpecifier ResultType { get;  }

        public override ValueCategory ValueCategory => ValueCategory.NoValue;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidCallExpression;
    }
}