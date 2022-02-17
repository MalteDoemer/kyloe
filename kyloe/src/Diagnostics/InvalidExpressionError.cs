using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class InvalidExpressionError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public InvalidExpressionError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.InvalidExpressionError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message()
        {
            return "Invalid expression.";
        }
    }
}