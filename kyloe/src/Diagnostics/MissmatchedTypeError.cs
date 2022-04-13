using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class MissmatchedTypeError : Diagnostic
    {
        private readonly SyntaxNode node;
        private readonly TypeSpecifier expected;
        private readonly TypeSpecifier provided;

        public MissmatchedTypeError(SyntaxNode node, TypeSpecifier expected, TypeSpecifier provided)
        {
            this.node = node;
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.MissmatchedTypeError;

        public override SourceLocation? Location => node.Location;

        public override string Message() => $"missmatched types, expected '{expected.FullName()}' got '{provided.FullName()}'";
    }
}