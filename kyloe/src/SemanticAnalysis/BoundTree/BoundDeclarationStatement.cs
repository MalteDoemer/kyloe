using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(LocalVariableSymbol variableSymbol, BoundTypeClause? typeClause, BoundExpression initializationExpression)
        {
            VariableSymbol = variableSymbol;
            TypeClause = typeClause;
            InitializationExpression = initializationExpression;
        }

        public LocalVariableSymbol VariableSymbol { get; }
        public BoundTypeClause? TypeClause { get; }
        public BoundExpression InitializationExpression { get; }

        public override BoundNodeType Type => BoundNodeType.BoundDeclarationStatement;
    }
}