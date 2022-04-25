using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class OverloadWithSameParametersExistsError : Diagnostic 
    {
        private readonly SyntaxTerminal nameToken;

        public OverloadWithSameParametersExistsError(SyntaxTerminal nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.OverloadWithSameParametersExistsError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"the function {nameToken.Text} with the same parameters already exists";
    }
}