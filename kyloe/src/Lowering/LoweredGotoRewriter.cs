using Kyloe.Symbols;
using static Kyloe.Lowering.LoweredNodeFactory;

namespace Kyloe.Lowering
{
    /// <summary>
    /// This class rewrites all if statements and while loops and other sort of jumps 
    /// into conditional and unconditional gotos.
    /// </summary>
    internal sealed class LoweredGotoRewriter : LoweredTreeRewriter
    {
        public LoweredGotoRewriter(TypeSystem typeSystem) : base(typeSystem)
        {
        }

        protected override LoweredStatement RewriteWhileStatement(LoweredWhileStatement whileLoop)
        {
            // while condition
            // continue:
            //      body
            // break:


            // goto check_condition
            //
            // continue:
            //      body
            // check_condition:
            //      goto continue if condition
            // break:
            // 


            var checkConditionLabel = LoweredLabel.Create();

            var block = Block(
                Goto(checkConditionLabel),
                LabelStatement(whileLoop.ContinueLabel),
                whileLoop.Body,
                LabelStatement(checkConditionLabel),
                GotoIf(whileLoop.ContinueLabel, whileLoop.Condition),
                LabelStatement(whileLoop.BreakLabel)
            );

            return RewriteStatement(block);
        }

        protected override LoweredStatement RewriteContinueStatement(LoweredContinueStatement statement)
        {
            return new LoweredGotoStatement(statement.Label);
        }

        protected override LoweredStatement RewriteBreakStatement(LoweredBreakStatement statement)
        {
            return new LoweredGotoStatement(statement.Label);
        }

        protected override LoweredStatement RewriteIfStatement(LoweredIfStatement statement)
        {
            if (statement.ElseStatment is LoweredEmptyStatement)
            {
                // if (condition) 
                //      body

                // goto end if not condition
                //      body
                // end:


                var endLabel = LoweredLabel.Create();

                var block = Block(
                    GotoIfNot(endLabel, statement.Condition),
                    statement.Body,
                    LabelStatement(endLabel)
                );

                return RewriteStatement(block);
            }
            else
            {
                // if (condition) 
                //      body
                // else
                //      elseBody

                // goto else if not condition
                //      body
                //      goto end
                // else:
                //      elseBody
                // end:

                var elseLabel = LoweredLabel.Create();
                var endLabel = LoweredLabel.Create();

                var block = Block(
                    GotoIfNot(elseLabel, statement.Condition),
                    statement.Body,
                    Goto(endLabel),
                    LabelStatement(elseLabel),
                    statement.ElseStatment,
                    LabelStatement(endLabel)
                );

                return RewriteStatement(block);
            }
        }

    }
}