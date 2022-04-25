using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedModifiableValueError : Diagnostic
    {
        private readonly SyntaxToken expression;

        public ExpectedModifiableValueError(SyntaxToken expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedModifiableValueError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expected a modifiable value";
    }
}