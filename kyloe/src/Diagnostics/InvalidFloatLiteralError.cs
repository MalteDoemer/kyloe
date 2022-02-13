using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{

    internal class InvalidFloatLiteralError : Diagnostic
    {
        private readonly SyntaxToken token;

        public InvalidFloatLiteralError(SyntaxToken token)
        {
            this.token = token;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.InvalidFloatLiteralError;

        public override SourceLocation? Location => token.Location;

        public override string Message() => "Invalid float literal.";
    }
}