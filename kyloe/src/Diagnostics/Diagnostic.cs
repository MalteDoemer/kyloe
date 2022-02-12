using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public abstract class Diagnostic
    {
        public abstract DiagnosticType Type { get; }

        public abstract SourceLocation? Location { get; }

        public abstract string Message();
    }
}