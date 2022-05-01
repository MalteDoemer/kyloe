using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundDeclarationStatement : BoundStatement
    {
        public BoundDeclarationStatement(Symbol variableSymbol, BoundExpression initializationExpression, SyntaxToken syntax)
        {
            VariableSymbol = variableSymbol;
            InitializationExpression = initializationExpression;
            Syntax = syntax;
        }

        public Symbol VariableSymbol { get; }
        public BoundExpression InitializationExpression { get; }
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundDeclarationStatement;
    }
}