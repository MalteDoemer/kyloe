using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class UnsupportedAssignmentOperation : Diagnostic
    {
        private readonly AssignmentExpression expression;
        private readonly ISymbol leftType;
        private readonly ISymbol rightType;

        public UnsupportedAssignmentOperation(AssignmentExpression expression, ISymbol leftType, ISymbol rightType)
        {
            this.expression = expression;
            this.leftType = leftType;
            this.rightType = rightType;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.UnsupportedAssignmentOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{SyntaxInfo.GetTokenTypeString(expression.OperatorToken.Type)}' cannot be used with types '{leftType}' and '{rightType}'";
    }
}