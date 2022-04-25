using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NameAlreadyExistsError : Diagnostic
    {
        private readonly SyntaxTerminal nameToken;

        public NameAlreadyExistsError(SyntaxTerminal nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NameAlreadyExistsError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"the name {nameToken.Text} already exists";
    }
}