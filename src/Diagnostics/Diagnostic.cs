using System;
using System.Linq;
using System.Collections.Generic;
using Kyloe.Syntax;

namespace Kyloe.Diagnostics
{
    enum DiagnosticType
    {
        Error,
        Warn,
    }

    abstract class Diagnostic
    {
        public abstract DiagnosticType Type { get; }

        public abstract string Message();
    }

    class DiagnosticCollecter
    {
        private readonly List<Diagnostic> diagnostics;

        public DiagnosticCollecter()
        {
            diagnostics = new List<Diagnostic>();
        }

        public void Add(Diagnostic diagnostic)
        {
            this.diagnostics.Add(diagnostic);
        }

        public IEnumerable<Diagnostic> GetAll() => diagnostics;
        public IEnumerable<Diagnostic> GetErrors() => diagnostics.FindAll(d => d.Type == DiagnosticType.Error);
        public IEnumerable<Diagnostic> GetWarnings() => diagnostics.FindAll(d => d.Type == DiagnosticType.Warn);

        public bool HasDiagnostics() => diagnostics.Count() != 0;
        public bool HasErrors() => GetErrors().Count() != 0;
        public bool HasWarnings() => GetWarnings().Count() != 0;
    }

    class UnexpectedTokenError : Diagnostic
    {
        private readonly SyntaxTokenType[] expected;
        private readonly SyntaxTokenType provided;

        public UnexpectedTokenError(SyntaxTokenType provided) : this(Array.Empty<SyntaxTokenType>(), provided)
        {
        }

        public UnexpectedTokenError(SyntaxTokenType[] expected, SyntaxTokenType provided)
        {
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override string Message()
        {
            if (expected.Length == 0)
                return $"Unexpected token '{provided}'.";
            else if (expected.Length == 1)
                return $"Expected '{expected[0]}', found '{provided}'.";
            else
                return $"Expected one of '{string.Join('|', expected)}' but found '{provided}'.";
        }
    }

    class NeverClosedStringLiteralError : Diagnostic
    {
        public override DiagnosticType Type => DiagnosticType.Error;

        public override string Message()
        {
            return "Never closed string literal.";
        }
    }

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