using System.Collections.Immutable;

namespace Kyloe.Semantics
{
    internal sealed class BoundCompilationUnit : BoundNode
    {
        public BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement> globals, ImmutableArray<BoundFunctionDefinition> functions, BoundFunctionDefinition? mainFunction)
        {
            Globals = globals;
            Functions = functions;
            MainFunction = mainFunction;
        }

        public ImmutableArray<BoundDeclarationStatement> Globals { get; }
        public ImmutableArray<BoundFunctionDefinition> Functions { get; }

        public BoundFunctionDefinition? MainFunction { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundCompilationUnit;
    }
}