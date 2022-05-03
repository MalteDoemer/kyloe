using Kyloe.Semantics;
using Kyloe.Symbols;
using static Kyloe.Lowering.LoweredNodeFactory;

namespace Kyloe.Lowering
{
    /// <summary>
    /// This class rewrites all if statements and while loops and other sort of jumps 
    /// into conditional and unconditional gotos.
    /// It also simplifies other statements such as assignment and declaration
    /// </summary>
    internal sealed class LoweredTreeSimplifier : LoweredTreeRewriter
    {
        public LoweredTreeSimplifier(TypeSystem typeSystem) : base(typeSystem)
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


            var checkConditionLabel = LoweredLabel.Create("check");

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


                var endLabel = LoweredLabel.Create("end");

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

                var elseLabel = LoweredLabel.Create("else");
                var endLabel = LoweredLabel.Create("end");

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

        protected override LoweredExpression RewriteAssignment(LoweredAssignment expression)
        {
            if (expression.Operation == AssignmentOperation.Assign)
                return base.RewriteAssignment(expression);

            // expr1 += expr2

            // expr1 = expr1 + expr2

            var left = expression.LeftExpression;
            var right = expression.RightExpression;
            var op = SemanticInfo.GetOperationForAssignment(expression.Operation);
            var binary = new LoweredBinaryExpression(left.Type, left, op, right);
            var assign = new LoweredAssignment(typeSystem, left, AssignmentOperation.Assign, binary);

            return base.RewriteAssignment(assign);
        }

        protected override LoweredStatement RewriteDeclarationStatement(LoweredDeclarationStatement statement)
        {
            if (statement.Initializer is not null)
            {
                // var x = 5;

                // var x;
                // x = 5;

                var decl = new LoweredDeclarationStatement(statement.Symbol, null);

                var initializer = RewriteExpression(statement.Initializer);
                var localAccess = new LoweredVariableAccessExpression(statement.Symbol);
                var assign = new LoweredAssignment(typeSystem, localAccess, AssignmentOperation.Assign, initializer);

                return Block(decl, ExpressionStatement(assign));
            }

            return base.RewriteDeclarationStatement(statement);
        }
    }
}