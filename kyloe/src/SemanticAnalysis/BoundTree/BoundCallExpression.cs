using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionCallExpression : BoundExpression
    {
        public BoundFunctionCallExpression(FunctionType function, BoundExpression expression, BoundArguments arguments)
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

        public override BoundNodeKind Kind => BoundNodeKind.BoundCallExpression;
    }
}