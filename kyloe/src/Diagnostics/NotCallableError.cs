using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NotCallableError : Diagnostic 
    {
        private readonly SyntaxToken expression;

        public NotCallableError(SyntaxToken expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NotCallableError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => "expression is not callable";
    }
}