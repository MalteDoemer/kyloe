using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(TypeSpecifier result, object value, SyntaxToken syntax)
        {
            ResultType = result;
            Value = value;
            Syntax = syntax;
        }

        public object Value { get; }
        
        public override SyntaxToken Syntax { get; }

        public override TypeSpecifier ResultType { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundLiteralExpression;

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;
    }
}