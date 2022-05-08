using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(TypeInfo functionType, TypeInfo resultType, BoundExpression expression, BoundArguments arguments, SyntaxToken syntax)
        {
            FunctionType = functionType;
            ResultType = resultType;
            Expression = expression;
            Arguments = arguments;
            Syntax = syntax;
        }

        public TypeInfo FunctionType { get; }
        public BoundExpression Expression { get; }
        public BoundArguments Arguments { get; }
        
        public override  SyntaxToken Syntax { get; }

        public override TypeInfo ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override BoundNodeKind Kind => BoundNodeKind.BoundCallExpression;
    }
}