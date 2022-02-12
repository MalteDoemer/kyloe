using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public abstract class Diagnostic
    {
        public abstract DiagnosticSeverity Severity { get; }

        public abstract SourceLocation? Location { get; }

        public abstract string Message();
    }
}