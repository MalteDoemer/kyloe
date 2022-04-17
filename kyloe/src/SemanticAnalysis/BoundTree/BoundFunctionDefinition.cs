using System.Collections.Immutable;
using Kyloe.Symbols;

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

    internal sealed class BoundFunctionDefinition : BoundNode
    {
        public BoundFunctionDefinition(FunctionType functionType, BoundBlockStatement body)
        {
            FunctionType = functionType;
            Body = body;
        }

        public FunctionType FunctionType { get; }
        public BoundBlockStatement Body { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BoundFunctionDefinition;
    }
}