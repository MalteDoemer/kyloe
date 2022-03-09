using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundLocalVariableExpression : BoundExpression
    {
        public BoundLocalVariableExpression(ILocalVariableSymbol variableSymbol)
        {
            VariableSymbol = variableSymbol;
        }

        public ILocalVariableSymbol VariableSymbol { get; }

        public override ISymbol ResultSymbol => VariableSymbol.Type;

        public override BoundNodeType Type => BoundNodeType.BoundLocalVariableExpression;

        public override bool IsValue => true;

    }

    internal class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(ITypeSymbol result, object value)
        {
            ResultType = result;
            Value = value;
        }

        public object Value { get; }
        public ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

        public override BoundNodeType Type => BoundNodeType.BoundLiteralExpression;

        public override bool IsValue => true;
    }
}