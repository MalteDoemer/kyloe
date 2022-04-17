using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(Symbol variableSymbol, BoundTypeClause? typeClause, BoundExpression initializationExpression)
        {
            VariableSymbol = variableSymbol;
            TypeClause = typeClause;
            InitializationExpression = initializationExpression;
        }

        public Symbol VariableSymbol { get; }
        public BoundTypeClause? TypeClause { get; }
        public BoundExpression InitializationExpression { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundDeclarationStatement;
    }
}