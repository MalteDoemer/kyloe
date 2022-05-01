using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(TypeSpecifier functionType, TypeSpecifier resultType, BoundExpression expression, BoundArguments arguments, SyntaxToken syntax)
        {
            FunctionType = functionType;
            ResultType = resultType;
            Expression = expression;
            Arguments = arguments;
            Syntax = syntax;
        }

        public TypeSpecifier FunctionType { get; }
        public BoundExpression Expression { get; }
        public BoundArguments Arguments { get; }
        
        public override  SyntaxToken Syntax { get; }

        public override TypeSpecifier ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override BoundNodeKind Kind => BoundNodeKind.BoundCallExpression;
    }
}