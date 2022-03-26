using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class RedefinedParameterError : Diagnostic
    {
        private readonly SyntaxToken nameToken;

        public RedefinedParameterError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.RedefinedParameterError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"parameter with the name '{nameToken.Value}' already exists";
    }
}