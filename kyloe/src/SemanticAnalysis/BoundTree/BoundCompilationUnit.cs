using System.Collections.Immutable;

namespace Kyloe.Semantics
{
    internal sealed class BoundCompilationUnit : BoundNode
    {
        public BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement> globals, ImmutableArray<BoundFunctionDefinition> functions)
        {
            Globals = globals;
            Functions = functions;
        }

        public ImmutableArray<BoundDeclarationStatement> Globals { get; }
        public ImmutableArray<BoundFunctionDefinition> Functions { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundCompilationUnit;
    }
}