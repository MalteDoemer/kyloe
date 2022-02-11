

using Kyloe.Syntax;
using Kyloe.Utility;

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