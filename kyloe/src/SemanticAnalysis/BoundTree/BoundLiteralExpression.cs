using Mono.Cecil;

namespace Kyloe.Semantics
{


    internal class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(TypeReference literalType, object value)
        {
            LiteralType = literalType;
            Value = value;
        }

        public TypeReference LiteralType { get; }
        public object Value { get; }

        public override BoundExpressionResult Result => new BoundExpressionResult(LiteralType, isInstance: true);

        public override BoundNodeType Type => BoundNodeType.BoundLiteralExpression;
    }
}