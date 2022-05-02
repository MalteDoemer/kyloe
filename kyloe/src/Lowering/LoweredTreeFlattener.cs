using System.Collections.Immutable;
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
            var flat = statements.ToImmutable();
            statements.Clear();
            return new LoweredFunctionDefinition(functionDefinition.Type, new LoweredBlockStatement(flat));
        }
    }
}