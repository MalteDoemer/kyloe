using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundFunctionDefinition : BoundNode
    {
        public BoundFunctionDefinition(BoundFunctionDeclaration declaration, BoundBlockStatement body, SyntaxToken syntax)
        {
            Declaration = declaration;
            Body = body;
            Syntax = syntax;
        }

        public BoundFunctionDeclaration Declaration { get; }
        public BoundBlockStatement Body { get; }

        public FunctionType Type => Declaration.Type;

        public override SyntaxToken Syntax { get; }
        
        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDefinition;
    }
}