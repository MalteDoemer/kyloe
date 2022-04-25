using Kyloe.Semantics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class UnsupportedUnaryOperation : Diagnostic
    {
        private readonly SyntaxToken expression;
        private readonly BoundOperation operation;
        private readonly TypeSpecifier type;

        public UnsupportedUnaryOperation(SyntaxToken expression, BoundOperation operation, TypeSpecifier type)
        {
            this.expression = expression;
            this.operation = operation;
            this.type = type;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.UnsupportedUnaryOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{operation}' cannot be used with type '{type.FullName()}'";
    }
}