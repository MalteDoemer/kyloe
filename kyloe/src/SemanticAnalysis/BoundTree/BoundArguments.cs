using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundArguments : BoundNode, IEnumerable<BoundExpression>
    {
        public BoundArguments(ImmutableArray<BoundExpression> arguments, SyntaxToken syntax)
        {
            Arguments = arguments;
            Syntax = syntax;
        }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public bool AllArgumentsValid => Arguments.All(a => a.ResultType is not ErrorType && a.IsValue);

        public IEnumerable<TypeInfo> ArgumentTypes => Arguments.Select(a => a.ResultType);

        public override BoundNodeKind Kind => BoundNodeKind.BoundArguments;
        public override SyntaxToken Syntax { get; }

        public IEnumerator<BoundExpression> GetEnumerator()
        {
            return ((IEnumerable<BoundExpression>)Arguments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Arguments).GetEnumerator();
        }
    }
}