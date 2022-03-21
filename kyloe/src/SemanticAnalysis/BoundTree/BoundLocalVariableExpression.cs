using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundLocalVariableExpression : BoundExpression
    {
        public BoundLocalVariableExpression(LocalVariableSymbol variableSymbol)
        {
            VariableSymbol = variableSymbol;
        }

        public LocalVariableSymbol VariableSymbol { get; }

        public override TypeSpecifier ResultType => VariableSymbol.Type;

        public override BoundNodeType Type => BoundNodeType.BoundLocalVariableExpression;

        public override ValueCategory ValueCategory => VariableSymbol.IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
    }
}