using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{

    internal sealed class InvalidFloatLiteralError : Diagnostic
    {
        private readonly SyntaxToken token;

        public InvalidFloatLiteralError(SyntaxToken token)
        {
            this.token = token;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.InvalidFloatLiteralError;

        public override SourceLocation? Location => token.Location;

        public override string Message() => "invalid float literal";
    }
}