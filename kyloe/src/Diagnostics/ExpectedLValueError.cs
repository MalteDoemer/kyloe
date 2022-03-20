using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedModifiableValueError : Diagnostic
    {
        private readonly SyntaxExpression expression;

        public ExpectedModifiableValueError(SyntaxExpression expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.ExpectedModifiableValueError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expected a modifiable value";
    }
}