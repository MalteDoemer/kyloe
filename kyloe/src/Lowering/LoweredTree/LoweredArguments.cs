using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Lowering
{
    internal sealed class LoweredArguments : LoweredNode, IEnumerable<LoweredExpression>
    {
        public LoweredArguments(ImmutableArray<LoweredExpression> arguments)
        {
            Arguments = arguments;
        }

        public ImmutableArray<LoweredExpression> Arguments { get; }

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredArguments;

        public IEnumerator<LoweredExpression> GetEnumerator()
        {
            return ((IEnumerable<LoweredExpression>)Arguments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Arguments).GetEnumerator();
        }
    }
}
