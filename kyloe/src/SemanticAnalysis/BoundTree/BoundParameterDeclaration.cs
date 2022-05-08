using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundParameterDeclaration : BoundNode
    {
        public BoundParameterDeclaration(ParameterSymbol symbol, SyntaxToken syntax)
        {
            Symbol = symbol;
            Syntax = syntax;
        }

        public ParameterSymbol Symbol { get; }

        public TypeInfo Type => Symbol.Type;

        public override SyntaxToken Syntax { get; }
        public override BoundNodeKind Kind => BoundNodeKind.BoundParameterDeclaration;
    }
}