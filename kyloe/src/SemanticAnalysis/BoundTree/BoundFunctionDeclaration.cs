using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDeclaration : BoundNode
    {
        public BoundFunctionDeclaration(FunctionType functionType, BoundParameters parameters, BoundTypeClause typeClause, SyntaxToken syntax)
        {
            FunctionType = functionType;
            Parameters = parameters;
            TypeClause = typeClause;
            Syntax = syntax;
        }

        public FunctionType FunctionType { get; }
        public BoundParameters Parameters { get; }
        public BoundTypeClause TypeClause { get; }
        
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDeclaration;
    }
}