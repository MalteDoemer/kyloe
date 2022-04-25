using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class RedefinedParameterError : Diagnostic
    {
        private readonly SyntaxTerminal nameToken;

        public RedefinedParameterError(SyntaxTerminal nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.RedefinedParameterError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"parameter with the name '{nameToken.Text}' already exists";
    }
}