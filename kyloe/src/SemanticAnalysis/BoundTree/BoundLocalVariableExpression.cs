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

        public override ISymbol ResultSymbol => VariableSymbol.Type;

        public override BoundNodeType Type => BoundNodeType.BoundLocalVariableExpression;

        public override ValueCategory ValueCategory => VariableSymbol.IsConst ? ValueCategory.RValue : ValueCategory.LValue;
    }
}