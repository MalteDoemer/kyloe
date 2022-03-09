using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedExpressionError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public ExpectedExpressionError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.ExpectedExpressionError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message() => "expected expression";
    }
}