using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class FunctionWithSameParametersExistsError : Diagnostic 
    {
        private readonly SyntaxToken nameToken;

        public FunctionWithSameParametersExistsError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.FunctionWithSameParametersExistsError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"the function {nameToken.Value} with the same parameters already exists";
    }
}