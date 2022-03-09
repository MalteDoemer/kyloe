using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics 
{
    internal sealed class RedefinedLocalVariableError : Diagnostic
    {
        private readonly SyntaxToken nameToken;

        public RedefinedLocalVariableError(SyntaxToken nameToken)
        {
            this.nameToken = nameToken;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.RedefinedLocalVariableError;

        public override SourceLocation? Location => nameToken.Location;

        public override string Message() => $"local variable with the name '{nameToken.Value}' already exists in this scope";
    }
}