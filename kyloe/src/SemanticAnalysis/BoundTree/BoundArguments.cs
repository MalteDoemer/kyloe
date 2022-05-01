using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundArguments : BoundNode
    {
        public BoundArguments(ImmutableArray<BoundExpression> arguments)
        {
            Arguments = arguments;
        }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public bool AllArgumentsValid => Arguments.All(a => a.ResultType is not ErrorType && a.IsValue);

        public IEnumerable<TypeSpecifier> ArgumentTypes => Arguments.Select(a => a.ResultType);

        public override BoundNodeKind Kind => BoundNodeKind.BoundArguments;
    }
}