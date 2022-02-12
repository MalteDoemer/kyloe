using System;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class UnexpectedTokenError : Diagnostic
    {
        private readonly SyntaxTokenType[] expected;
        private readonly SyntaxToken provided;

        public UnexpectedTokenError(SyntaxToken provided) : this(Array.Empty<SyntaxTokenType>(), provided)
        {
        }

        public UnexpectedTokenError(SyntaxTokenType[] expected, SyntaxToken provided)
        {
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticType Type => DiagnosticType.Error;

        public override SourceLocation? Location => provided.Location;

        public override string Message()
        {
            if (expected.Length == 0)
                return $"Unexpected token '{provided.Type}'.";
            else if (expected.Length == 1)
                return $"Expected '{expected[0]}', found '{provided.Type}'.";
            else
                return $"Expected one of '{string.Join('|', expected)}' but found '{provided.Type}'.";
        }
    }
}