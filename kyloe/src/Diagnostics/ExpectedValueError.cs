using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedValueError : Diagnostic
    {
        private readonly SyntaxExpression expression;

        public ExpectedValueError(SyntaxExpression expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedValueError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expected a value";
    }
}