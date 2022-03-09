using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics 
{
    internal sealed class MissmatchedTypeError : Diagnostic
    {
        private readonly SyntaxExpression expression;
        private readonly ISymbol expected;
        private readonly ISymbol provided;

        public MissmatchedTypeError(SyntaxExpression expression, ISymbol expected, ISymbol provided)
        {
            this.expression = expression;
            this.expected = expected;
            this.provided = provided;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticType Type => DiagnosticType.MissmatchedTypeError;

        public override SourceLocation? Location => expression.Location;

        public override string Message() => $"missmatched types, expected '{expected}' got '{provided}'";
    }
}