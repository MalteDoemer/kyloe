using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedValueError : Diagnostic
    {
        private readonly SyntaxNode node;

        public ExpectedValueError(SyntaxNode node)
        {
            this.node = node;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedValueError;

        public override SourceLocation? Location => node.Location;

        public override string Message() => "expected a value";
    }
}