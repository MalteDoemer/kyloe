using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(ILocalVariableSymbol variableSymbol, BoundTypeClause? typeClause, BoundExpression initializationExpression)
        {
            VariableSymbol = variableSymbol;
            TypeClause = typeClause;
            InitializationExpression = initializationExpression;
        }

        public ILocalVariableSymbol VariableSymbol { get; }
        public BoundTypeClause? TypeClause { get; }
        public BoundExpression InitializationExpression { get; }

        public override BoundNodeType Type => BoundNodeType.BoundDeclarationStatement;
    }
}