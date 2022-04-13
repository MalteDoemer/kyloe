using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedModifiableValueError : Diagnostic
    {
        private readonly SyntaxNode node;

        public ExpectedModifiableValueError(SyntaxNode node)
        {
            this.node = node;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedModifiableValueError;

        public override SourceLocation? Location => node.Location;

        public override string Message() => "expected a modifiable value";
    }
}