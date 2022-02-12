

using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class NeverClosedBlockComment : Diagnostic
    {
        public NeverClosedBlockComment(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override SourceLocation? Location => Token.Location;

        public override string Message() => "Never closed block comment.";
    }
}