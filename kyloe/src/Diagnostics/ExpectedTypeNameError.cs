using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class ExpectedTypeNameError : Diagnostic
    {
        private readonly SyntaxNode node;

        public ExpectedTypeNameError(SyntaxNode node)
        {
            this.node = node;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.ExpectedTypeNameError;

        public override SourceLocation? Location => node.Location;

        public override string Message() => "expected a type name";
    }
}