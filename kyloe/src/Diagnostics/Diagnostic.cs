using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public sealed class Diagnostic
    {
        public Diagnostic(DiagnosticSeverity severity, DiagnosticKind kind, string message, SourceLocation? location)
        {
            Severity = severity;
            Kind = kind;
            Message = message;
            Location = location;
        }

        public DiagnosticSeverity Severity { get; }

        public DiagnosticKind Kind { get; }

        public string Message { get; }

        public SourceLocation? Location { get; }
    }
}