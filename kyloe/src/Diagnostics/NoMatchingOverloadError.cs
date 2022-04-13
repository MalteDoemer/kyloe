using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NoMatchingOverloadError : Diagnostic
    {
        private readonly string name;
        private readonly CallSyntax callSyntax;
        private readonly BoundArguments arguments;

        public NoMatchingOverloadError(string name, CallSyntax callExpression, BoundArguments arguments)
        {
            this.name = name;
            this.callSyntax = callExpression;
            this.arguments = arguments;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NoMatchingOverloadError;

        public override SourceLocation? Location => callSyntax.Location;

        public override string Message()
        {
            if (arguments.Arguments.Length == 0)
                return $"cannot call {name} without arguments";
            else
                return $"cannot call {name} with arguments ({arguments.JoinArgumentTypes()})";
        }
    }
}