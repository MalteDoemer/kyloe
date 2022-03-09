using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedLValueError : Diagnostic
    {
        private readonly SyntaxExpression expression;

        public ExpectedLValueError(SyntaxExpression expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.ExpectedLValueError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expected an lvalue";
    }
}