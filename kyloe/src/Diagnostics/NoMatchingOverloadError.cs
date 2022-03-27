using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class NoMatchingOverloadError : Diagnostic
    {
        private readonly string name;
        private readonly CallExpression callExpression;
        private readonly BoundArguments arguments;

        public NoMatchingOverloadError(string name, CallExpression callExpression, BoundArguments arguments)
        {
            this.name = name;
            this.callExpression = callExpression;
            this.arguments = arguments;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NoMatchingOverloadError;

        public override SourceLocation? Location => callExpression.Location;

        public override string Message()
        {
            if (arguments.Arguments.Length == 0)
                return $"cannot call {name} without arguments";
            else
                return $"cannot call {name} with arguments ({arguments.JoinArgumentTypes()})";
        }
    }
}