using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class MissmatchedTypeError : Diagnostic
    {
        private readonly SyntaxToken expression;
        private readonly TypeSpecifier expected;
        private readonly TypeSpecifier provided;

        public MissmatchedTypeError(SyntaxToken expression, TypeSpecifier expected, TypeSpecifier provided)
        {
            this.expression = expression;
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.MissmatchedTypeError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"missmatched types, expected '{expected.FullName()}' got '{provided.FullName()}'";
    }
}