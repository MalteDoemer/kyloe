using Kyloe.Text;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    class NeverClosedStringLiteralError : Diagnostic
    {
        private readonly SyntaxToken token;

        public NeverClosedStringLiteralError(SyntaxToken token)
        {
            this.token = token;
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override SourceLocation? Location => token.Location;

        public override string Message()
        {
            return "Never closed string literal.";
        }
    }
}