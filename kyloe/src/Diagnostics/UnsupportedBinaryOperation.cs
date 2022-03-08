using Kyloe.Semantics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;
using Mono.Cecil;

namespace Kyloe.Diagnostics
{
    internal class UnsupportedBinaryOperation : Diagnostic
    {
        private readonly BinaryExpression expression;
        private readonly ISymbol leftType;
        private readonly ISymbol rightType;

        public UnsupportedBinaryOperation(BinaryExpression expression, ISymbol leftType, ISymbol rightType)
        {
            this.leftType = leftType;
            this.expression = expression;
            this.rightType = rightType;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.UnsupportedBinaryOperation;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"operator '{SyntaxInfo.GetTokenTypeString(expression.OperatorToken.Type)}' cannot be used with types '{leftType}' and  '{rightType}'";
    }
}