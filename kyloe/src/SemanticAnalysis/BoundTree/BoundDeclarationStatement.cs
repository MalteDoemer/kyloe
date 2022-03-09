using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(ILocalVariableSymbol variableSymbol, BoundExpression initializationExpression)
        {
            VariableSymbol = variableSymbol;
            InitializationExpression = initializationExpression;
        }

        public ILocalVariableSymbol VariableSymbol { get; }
        public BoundExpression InitializationExpression { get; }

        public override BoundNodeType Type => BoundNodeType.BoundDeclarationStatement;
    }
}