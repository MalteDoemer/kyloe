

using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class NeverClosedBlockCommentError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public NeverClosedBlockCommentError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.NeverClosedBlockCommentError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message() => "never closed block comment";
    }
}