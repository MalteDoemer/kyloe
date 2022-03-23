using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class MemberAccessError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly TypeSpecifier resultType;
        private readonly string memberName;

        public MemberAccessError(SyntaxExpression expression, TypeSpecifier resultType, string memberName)
        {
            this.expression = expression;
            this.resultType = resultType;
            this.memberName = memberName;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.MemberAccessError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"cannot access the member '{memberName}' from '{resultType.FullName()}'";
    }
}