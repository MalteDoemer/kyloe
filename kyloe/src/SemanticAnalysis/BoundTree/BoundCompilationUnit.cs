using System.Collections.Immutable;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundCompilationUnit : BoundNode
    {
        public BoundCompilationUnit(ImmutableArray<BoundDeclarationStatement> globals, ImmutableArray<BoundFunctionDefinition> functions, BoundFunctionDefinition? mainFunction, SyntaxToken syntax)
        {
            Globals = globals;
            Functions = functions;
            MainFunction = mainFunction;
            Syntax = syntax;
        }

        public ImmutableArray<BoundDeclarationStatement> Globals { get; }
        public ImmutableArray<BoundFunctionDefinition> Functions { get; }

        public BoundFunctionDefinition? MainFunction { get; }
        public override SyntaxToken Syntax { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundCompilationUnit;
    }
}