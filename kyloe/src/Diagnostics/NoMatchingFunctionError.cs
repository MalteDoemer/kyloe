using Kyloe.Semantics;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class InstanceRequiredToCallError : Diagnostic
    {
        private readonly CallExpression callExpression;

        public InstanceRequiredToCallError(CallExpression callExpression)
        {
            this.callExpression = callExpression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.InstanceRequiredToCallError;

        public override SourceLocation? Location => callExpression.Location;

        public override string Message() => "a instance is required to call this method";
    }

    internal sealed class CannotCallStaticMethodError : Diagnostic
    {
        private readonly CallExpression callExpression;

        public CannotCallStaticMethodError(CallExpression callExpression)
        {
            this.callExpression = callExpression;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.CannotCallStaticMethodError;

        public override SourceLocation? Location => callExpression.Location;

        public override string Message() => "static function cannot be accessed with an instance";
    }

    internal sealed class NoMatchingFunctionError : Diagnostic
    {
        private readonly string name;
        private readonly CallExpression callExpression;
        private readonly BoundArguments arguments;

        public NoMatchingFunctionError(string name, CallExpression callExpression, BoundArguments arguments)
        {
            this.name = name;
            this.callExpression = callExpression;
            this.arguments = arguments;
        }

        public override DiagnosticSeverity Severity => DiagnosticSeverity.Error;

        public override DiagnosticKind Kind => DiagnosticKind.NoMatchingFunctionError;

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