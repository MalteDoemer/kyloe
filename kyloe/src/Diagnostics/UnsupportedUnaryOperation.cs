using Kyloe.Semantics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class UnsupportedUnaryOperation : Diagnostic
    {
        private readonly UnaryExpression expression;
        private readonly TypeSpecifier type;

        public UnsupportedUnaryOperation(UnaryExpression expression, TypeSpecifier type)
        {
            this.expression = expression;
            this.type = type;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.UnsupportedUnaryOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{SyntaxInfo.GetTokenKindString(expression.OperatorToken.Kind)}' cannot be used with type '{type.FullName()}'";
    }
}