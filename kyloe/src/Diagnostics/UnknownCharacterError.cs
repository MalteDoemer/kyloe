using Kyloe.Text;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    class UnknownCharacterError : Diagnostic
    {
        private readonly SyntaxToken token;

        public UnknownCharacterError(SyntaxToken errorToken)
        {
            this.token = errorToken;

            if (errorToken.Value is not char) {
                throw new System.ArgumentException("Token must have a character!");
            }
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