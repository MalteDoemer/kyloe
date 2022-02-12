using System.Diagnostics;

using Kyloe.Utility;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    internal class UnknownCharacterError : Diagnostic
    {
        private readonly SyntaxToken token;

        public UnknownCharacterError(SyntaxToken errorToken)
        {
            this.token = errorToken;
            Debug.Assert(errorToken.Value is char, "errorToken.Value must be a char");
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override SourceLocation? Location => token.Location;

        public override string Message()
        {
            char value = (char)token.Value!;
            return string.Format("Unknown character: \\u{0:x4}", (int)value);
        }
    }
}