using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Lowering
{
    internal sealed class LoweredCompilationUnit : LoweredNode 
    {
        public LoweredCompilationUnit(ImmutableArray<LoweredFunctionDefinition> loweredFunctions, LoweredStatement globalStatement, int mainFunctionIndex)
        {
            LoweredFunctions = loweredFunctions;
            GlobalStatement = globalStatement;
            MainFunctionIndex = mainFunctionIndex;
        }

        public ImmutableArray<LoweredFunctionDefinition> LoweredFunctions { get; }
        public LoweredStatement GlobalStatement { get; }
        
        public int MainFunctionIndex { get; }
        public LoweredFunctionDefinition? MainFunction => MainFunctionIndex < 0 ? null : LoweredFunctions[MainFunctionIndex];

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredCompilationUnit;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return GlobalStatement;
            foreach (var fn in LoweredFunctions)
                yield return fn;
        }
    }
}
