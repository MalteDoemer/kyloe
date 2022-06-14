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

        public override LoweredCompilationUnit RewriteCompilationUnit(LoweredCompilationUnit compilationUnit)
        {
            var globals = ImmutableArray.CreateBuilder<LoweredStatement>();
            var functions = ImmutableArray.CreateBuilder<LoweredFunctionDefinition>(compilationUnit.LoweredFunctions.Length);

            foreach (var func in compilationUnit.LoweredFunctions)
                functions.Add(RewriteFunctionDefinition(func));

            RewriteBlockStatement(compilationUnit.GlobalStatement);

            if (statements.Count == 0 || CanFallThrough(statements.Last()))
                statements.Add(new LoweredReturnStatement(null));

            var global = new LoweredBlockStatement(statements.ToImmutable());
            statements.Clear();

            return new LoweredCompilationUnit(functions.MoveToImmutable(), global, compilationUnit.MainFunctionIndex);
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

        protected override LoweredExpression RewriteStatementExpression(LoweredStatementExpression expression)
        {

            RewriteBlockStatement(expression.Statements);

            return expression.FinalExpression;
        }

        private bool CanFallThrough(LoweredStatement stmt)
        {
            return stmt.Kind != LoweredNodeKind.LoweredReturnStatement
                   && stmt.Kind != LoweredNodeKind.LoweredConditionalGotoStatement
                   && stmt.Kind != LoweredNodeKind.LoweredGotoStatement;
        }
    }
}