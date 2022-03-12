using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class MemberAccessError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly ISymbol resultSymbol;
        private readonly string memberName;

        public MemberAccessError(SyntaxExpression expression, ISymbol resultSymbol, string memberName)
        {
            this.expression = expression;
            this.resultSymbol = resultSymbol;
            this.memberName = memberName;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.MemberAccessError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"cannot access the member '{memberName}' from '{resultSymbol}' in this context";
    }

    internal sealed class MemberNotFoundError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly ISymbol resultSymbol;
        private readonly string memberName;

        public MemberNotFoundError(SyntaxExpression expression, ISymbol resultSymbol, string memberName)
        {
            this.expression = expression;
            this.resultSymbol = resultSymbol;
            this.memberName = memberName;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.MemberNotFoundError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"'{resultSymbol}' has no member with the name '{memberName}'";
    }
}