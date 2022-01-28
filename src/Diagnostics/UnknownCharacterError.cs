namespace Kyloe.Diagnostics
{
    class UnknownCharacterError : Diagnostic
    {
        private readonly char Character;

        public UnknownCharacterError(char character)
        {
            Character = character;
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override string Message()
        {
            return string.Format("Unknown character: \\u{0:x4}", (int)Character);
        }
    }
}