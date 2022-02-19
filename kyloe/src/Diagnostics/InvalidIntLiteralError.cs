using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{

    internal class InvalidIntLiteralError : Diagnostic
    {
        private readonly SyntaxToken token;

        public InvalidIntLiteralError(SyntaxToken token)
        {
            this.token = token;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.InvalidIntLiteralError;

        public override SourceLocation? Location => token.Location;

        public override string Message() => "Invalid int literal.";
    }
}