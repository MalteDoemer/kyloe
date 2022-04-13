using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NotCallableError : Diagnostic 
    {
        private readonly SyntaxNode node;

        public NotCallableError(SyntaxNode node)
        {
            this.node = node;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NotCallableError;

        public override SourceLocation? Location => node.Location;

        public override string Message() => "expression is not callable";
    }
}