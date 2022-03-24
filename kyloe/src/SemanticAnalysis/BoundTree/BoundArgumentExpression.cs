using System.Collections.Immutable;

namespace Kyloe.Semantics
{
    internal sealed class BoundArgumentExpression 
    {
        public BoundArgumentExpression(ImmutableArray<BoundExpression> arguments)
        {
            Arguments = arguments;
        }

        public ImmutableArray<BoundExpression> Arguments { get;  }
    }
}