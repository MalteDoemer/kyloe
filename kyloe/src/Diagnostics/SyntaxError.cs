using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public sealed class SyntaxError : Diagnostic
    {
        private readonly string message;
        public SyntaxError(DiagnosticKind kind, string message, SourceLocation location)
        {
            this.message = message;
            Kind = kind;
            Location = location;
        }

        public override DiagnosticKind Kind { get; }
        public override SourceLocation? Location { get; }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override string Message() => message;
    }

    // public enum SyntaxErrorKind
    // {
    //     UnexpectedTokenError,
    //     InvalidCharacterError,
    // }
}