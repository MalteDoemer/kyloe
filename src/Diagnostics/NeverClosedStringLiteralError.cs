namespace Kyloe.Diagnostics
{
    class NeverClosedStringLiteralError : Diagnostic
    {
        public override DiagnosticType Type => DiagnosticType.Error;

        public override string Message()
        {
            return "Never closed string literal.";
        }
    }
}