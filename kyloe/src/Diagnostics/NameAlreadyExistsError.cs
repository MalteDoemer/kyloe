using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NameAlreadyExistsError : Diagnostic
    {
        private readonly SyntaxToken nameToken;

        public NameAlreadyExistsError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NameAlreadyExistsError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"the name {nameToken.Value} already exists";
    }
}