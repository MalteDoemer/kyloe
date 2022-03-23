

using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NeverClosedBlockCommentError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public NeverClosedBlockCommentError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NeverClosedBlockCommentError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message() => "never closed block comment";
    }
}