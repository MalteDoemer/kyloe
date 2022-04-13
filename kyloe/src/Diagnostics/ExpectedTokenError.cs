using System;
using System.Linq;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedTokenError : Diagnostic
    {
        private readonly SyntaxTokenKind expected;
        private readonly SyntaxToken provided;

        public ExpectedTokenError(SyntaxTokenKind expected, SyntaxToken provided)
        {
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedTokenError;

        public override SourceLocation? Location => provided.Location;

        public override string Message()
        {
            return $"expected '{SyntaxInfo.GetTokenKindStringOrName(expected)}'";
        }
    }
}