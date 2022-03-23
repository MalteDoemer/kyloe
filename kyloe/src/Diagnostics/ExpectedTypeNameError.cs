using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedTypeNameError : Diagnostic
    {
        private readonly SyntaxExpression expression;

        public ExpectedTypeNameError(SyntaxExpression expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedTypeNameError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expected a type name";
    }
}