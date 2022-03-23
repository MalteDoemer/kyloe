using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class MemberAccessNotAllowed : Diagnostic
    {
        private readonly SyntaxExpression expression;

        public MemberAccessNotAllowed(SyntaxExpression expression)
        {
            this.expression = expression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.MemberAccessNotAllowed;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"cannot use the '.' operator on this expression";
    }
}