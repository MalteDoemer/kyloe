

using Kyloe.Syntax;
using Kyloe.Text;

namespace Kyloe.Diagnostics
{
    class NeverClosedBlockComment : Diagnostic
    {
        public NeverClosedBlockComment(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override SourceLocation? Location => Token.Location;

        public override string Message() => "Never closed block comment.";
    }
}