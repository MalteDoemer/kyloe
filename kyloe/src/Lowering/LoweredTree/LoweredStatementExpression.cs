using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredStatementExpression : LoweredExpression
    {
        public LoweredBlockStatement Statements;
        public LoweredExpression FinalExpression;

        public LoweredStatementExpression(LoweredBlockStatement statements, LoweredExpression finalExpression)
        {
            Statements = statements;
            FinalExpression = finalExpression;
        }

        public override TypeInfo Type => FinalExpression.Type;

        public override ValueCategory ValueCategory => FinalExpression.ValueCategory;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredStatementExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield return Statements;
            yield return FinalExpression;
        }
    }
}
