using System;
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
                return $"Unexpected token '{provided}'";
            else if (expected.Length == 1)
                return $"Expected '{expected[0]}', found '{provided}'";
            else
                return $"Expected one of '{string.Join('|', expected)}' but found '{provided}'";
        }
    }
}