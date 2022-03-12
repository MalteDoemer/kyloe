using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class RestrictedMemberAccessError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly ISymbol resultSymbol;

        public RestrictedMemberAccessError(SyntaxExpression expression, ISymbol resultSymbol)
        {
            this.expression = expression;
            this.resultSymbol = resultSymbol;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.RestrictedMemberAccessError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"cannot use the '.' operator on '{resultSymbol}' in this context";
    }
}