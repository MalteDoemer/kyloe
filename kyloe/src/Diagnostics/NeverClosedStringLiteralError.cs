using Kyloe.Utility;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    internal class NeverClosedStringLiteralError : Diagnostic
    {
        private readonly SyntaxToken token;

        public NeverClosedStringLiteralError(SyntaxToken token)
        {
            this.token = token;
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override SourceLocation? Location => token.Location;

        public override string Message() => "Never closed string literal.";
    }
}