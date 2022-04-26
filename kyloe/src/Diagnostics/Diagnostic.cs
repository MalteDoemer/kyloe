using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public sealed class Diagnostic
    {
        public Diagnostic(DiagnosticKind kind, string message, SourceLocation? location)
        {
            Kind = kind;
            Message = message;
            Location = location;
        }

        public DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public DiagnosticKind Kind { get; }

        public string Message { get; }

        public SourceLocation? Location { get; }
    }
}