using System.Collections.Immutable;
using System.Linq;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredTreeFlattener : LoweredTreeRewriter
    {
        private readonly ImmutableArray<LoweredStatement>.Builder statements;

        public LoweredTreeFlattener(TypeSystem typeSystem) : base(typeSystem)
        {
            statements = ImmutableArray.CreateBuilder<LoweredStatement>();
        }

        protected override LoweredBlockStatement RewriteBlockStatement(LoweredBlockStatement statement)
        {
            foreach (var stmt in statement)
            {
                var rewrite = RewriteStatement(stmt);

                if (rewrite is not LoweredBlockStatement)
                    statements.Add(rewrite);
            }

            return statement;
        }

        protected override LoweredFunctionDefinition RewriteFunctionDefinition(LoweredFunctionDefinition functionDefinition)
        {
            RewriteBlockStatement(functionDefinition.Body);

            if (statements.Count == 0 || CanFallThrough(statements.Last()))
                statements.Add(new LoweredReturnStatement(null));

            var flat = statements.ToImmutable();
            statements.Clear();
            return new LoweredFunctionDefinition(functionDefinition.Type, new LoweredBlockStatement(flat));
        }

        private bool CanFallThrough(LoweredStatement stmt)
        {
            return stmt.Kind != LoweredNodeKind.LoweredReturnStatement
                   && stmt.Kind != LoweredNodeKind.LoweredConditionalGotoStatement
                   && stmt.Kind != LoweredNodeKind.LoweredGotoStatement;
        }
    }
}