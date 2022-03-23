using System.Diagnostics;

using Kyloe.Utility;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    internal sealed class UnknownCharacterError : Diagnostic
    {
        private readonly SyntaxToken errorToken;

        public UnknownCharacterError(SyntaxToken errorToken)
        {
            this.errorToken = errorToken;
            Debug.Assert(errorToken.Value is char, "errorToken.Value must be a char");
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.UnknownCharacterError;

        public override SourceLocation? Location => errorToken.Location;

        public override string Message()
        {
            char value = (char)errorToken.Value!;
            return string.Format("unknown character: \\u{0:x4}", (int)value);
        }
    }
}