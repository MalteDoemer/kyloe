using Kyloe.Utility;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    internal sealed class NeverClosedStringLiteralError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public NeverClosedStringLiteralError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NeverClosedStringLiteralError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message() => "never closed string literal";
    }
}