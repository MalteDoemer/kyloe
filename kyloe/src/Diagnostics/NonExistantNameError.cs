using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics 
{
    internal sealed class NonExistantNameError : Diagnostic
    {   
        private readonly SyntaxToken nameToken;

        public NonExistantNameError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NonExistantNameError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"cannot find the name '{nameToken.Value}' in this scope";
    }
}