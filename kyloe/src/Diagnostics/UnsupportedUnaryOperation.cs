using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class UnsupportedUnaryOperation : Diagnostic
    {
        private readonly UnaryExpression expression;
        private readonly BoundResultType type;

        public UnsupportedUnaryOperation(UnaryExpression expression, BoundResultType type)
        {
            this.expression = expression;
            this.type = type;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.UnsupportedUnaryOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{SyntaxInfo.GetTokenTypeString(expression.OperatorToken.Type)}' cannot be used with type '{type}'";
    }
}