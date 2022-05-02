using System.Collections.Immutable;

namespace Kyloe.Lowering
{
    internal sealed class LoweredCompilationUnit : LoweredNode 
    {
        public LoweredCompilationUnit(ImmutableArray<LoweredFunctionDefinition> loweredFunctions, LoweredStatement globalStatement, LoweredFunctionDefinition? mainFunction)
        {
            LoweredFunctions = loweredFunctions;
            GlobalStatement = globalStatement;
            MainFunction = mainFunction;
        }

        public ImmutableArray<LoweredFunctionDefinition> LoweredFunctions { get; }
        public LoweredStatement GlobalStatement { get; }

        public LoweredFunctionDefinition? MainFunction { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredCompilationUnit;
    }
}
