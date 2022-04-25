using Kyloe.Semantics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class UnsupportedAssignmentOperation : Diagnostic
    {
        private readonly SyntaxToken expression;
        private readonly BoundOperation operation;
        private readonly TypeSpecifier leftType;
        private readonly TypeSpecifier rightType;

        public UnsupportedAssignmentOperation(SyntaxToken expression, BoundOperation operation, TypeSpecifier leftType, TypeSpecifier rightType)
        {
            this.expression = expression;
            this.operation = operation;
            this.leftType = leftType;
            this.rightType = rightType;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.UnsupportedAssignmentOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{operation}' cannot be used with types '{leftType.FullName()}' and '{rightType.FullName()}'";
    }
}