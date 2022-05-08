using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDeclaration : BoundNode
    {
        public BoundFunctionDeclaration(FunctionTypeInfo type, BoundParameters parameters, BoundTypeClause typeClause, SyntaxToken syntax)
        {
            Type = type;
            Parameters = parameters;
            TypeClause = typeClause;
            Syntax = syntax;
        }

        public FunctionTypeInfo Type { get; }
        public BoundParameters Parameters { get; }
        public BoundTypeClause TypeClause { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDeclaration;
    }
}