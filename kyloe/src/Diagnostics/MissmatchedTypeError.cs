using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics 
{
    internal sealed class MissmatchedTypeError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly ITypeSymbol expectedType;
        private readonly ISymbol provided;

        public MissmatchedTypeError(SyntaxExpression expression, ITypeSymbol expectedType, ISymbol provided)
        {
            this.expression = expression;
            this.expectedType = expectedType;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.MissmatchedTypeError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"expected '{expectedType}' got '{provided}'";
    }
}