using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class OverloadWithSameParametersExistsError : Diagnostic 
    {
        private readonly SyntaxToken nameToken;

        public OverloadWithSameParametersExistsError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.OverloadWithSameParametersExistsError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"the function {nameToken.Value} with the same parameters already exists";
    }
}