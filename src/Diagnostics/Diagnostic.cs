namespace Kyloe.Diagnostics
{
    abstract class Diagnostic
    {
        public abstract DiagnosticType Type { get; }

        public abstract string Message();
    }
}