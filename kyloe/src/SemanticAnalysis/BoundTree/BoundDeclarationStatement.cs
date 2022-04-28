using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(Symbol variableSymbol, BoundExpression initializationExpression)
        {
            VariableSymbol = variableSymbol;
            InitializationExpression = initializationExpression;
        }

        public Symbol VariableSymbol { get; }
        public BoundExpression InitializationExpression { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundDeclarationStatement;
    }
}