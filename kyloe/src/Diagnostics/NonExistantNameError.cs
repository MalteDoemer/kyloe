using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics 
{
    internal sealed class NonExistantNameError : Diagnostic
    {   
        private readonly SyntaxTerminal nameToken;

        public NonExistantNameError(SyntaxTerminal nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NonExistantNameError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"cannot find the name '{nameToken.Text}' in this scope";
    }
}