using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class UnmatchedClosingBracket : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public UnmatchedClosingBracket(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.UnmatchedClosingBracket;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message() => "Missmatched closing bracket.";
    }
}