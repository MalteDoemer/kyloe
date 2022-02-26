using Mono.Cecil;

namespace Kyloe.Semantics
{
    internal class BoundLiteralExpression : BoundExpression
    {
        private readonly BoundResultType resultType;

        public BoundLiteralExpression(TypeReference literalType, object value)
        {
            resultType = new BoundResultType(literalType, isValue: true);
            Value = value;
        }

        public object Value { get; }

        public override BoundResultType Result => resultType;

        public override BoundNodeType Type => BoundNodeType.BoundLiteralExpression;
    }
}