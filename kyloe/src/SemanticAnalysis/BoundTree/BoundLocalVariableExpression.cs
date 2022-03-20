using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundLocalVariableExpression : BoundExpression
    {
        public BoundLocalVariableExpression(ILocalVariableSymbol variableSymbol)
        {
            VariableSymbol = variableSymbol;
        }

        public ILocalVariableSymbol VariableSymbol { get; }

        public override ITypeSymbol ResultType => VariableSymbol.Type;

        public override ISymbol ResultSymbol => VariableSymbol;

        public override BoundNodeType Type => BoundNodeType.BoundLocalVariableExpression;

        public override ValueCategory ValueCategory => VariableSymbol.IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }
}