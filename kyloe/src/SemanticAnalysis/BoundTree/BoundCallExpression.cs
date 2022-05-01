using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionType function, BoundExpression expression, BoundArguments arguments, SyntaxToken syntax)
        {
            Function = function;
            Expression = expression;
            Arguments = arguments;
            Syntax = syntax;
        }

        public FunctionType Function { get; }
        public BoundExpression Expression { get; }
        public BoundArguments Arguments { get; }
        
        public override  SyntaxToken Syntax { get; }

        public override TypeSpecifier ResultType => Function.ReturnType;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override BoundNodeKind Kind => BoundNodeKind.BoundCallExpression;
    }
}