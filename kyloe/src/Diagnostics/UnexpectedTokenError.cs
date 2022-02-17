using System;
using System.Linq;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class UnexpectedTokenError : Diagnostic
    {
        private readonly SyntaxTokenType expected;
        private readonly SyntaxToken provided;

        public UnexpectedTokenError(SyntaxTokenType expected, SyntaxToken provided)
        {
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.UnexpectedTokenError;

        public override SourceLocation? Location => provided.Location;

        public override string Message()
        {
            string NameOrSymbol(SyntaxTokenType type)
            {
                if (SyntaxInfo.GetTokenTypeString(type) is string s)
                    return s;
                else
                    return type.ToString();
            }

            return $"Expected '{NameOrSymbol(expected)}'.";
        }
    }
}