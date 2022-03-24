using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Kyloe.Semantics
{
    internal sealed class BoundArguments 
    {
        public BoundArguments(ImmutableArray<BoundExpression> arguments)
        {
            Arguments = arguments;
        }

        public ImmutableArray<BoundExpression> Arguments { get;  }

        public string JoinArgumentTypes() 
        {
            var builder = new StringBuilder();
            builder.AppendJoin(", ", Arguments.Select(arg => arg.ResultType.FullName()));

            return builder.ToString();
        }
    }
}