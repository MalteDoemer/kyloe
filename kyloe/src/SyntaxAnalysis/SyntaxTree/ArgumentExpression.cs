using System.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Kyloe.Utility;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal sealed class ArgumentExpression
    {
        public ArgumentExpression(ImmutableArray<SyntaxExpression> nodes, ImmutableArray<SyntaxToken> commas)
        {
            Nodes = nodes;
            Commas = commas;

            Debug.Assert(Nodes.Length == 0 || Nodes.Length == Commas.Length + 1, "Wrong amount of commas passed to a ArgumentExpression");
        }

        public ImmutableArray<SyntaxExpression> Nodes { get; }
        public ImmutableArray<SyntaxToken> Commas { get; }

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            for (int i = 0; i < Commas.Length; i++)
            {
                yield return new SyntaxNodeChild(Nodes[i]);
                yield return new SyntaxNodeChild(Commas[i]);
            }

            if (Nodes.Length > 0)
                yield return new SyntaxNodeChild(Nodes.Last());
        }

        public static ArgumentExpression Empty = new ArgumentExpression(ImmutableArray<SyntaxExpression>.Empty, ImmutableArray<SyntaxToken>.Empty);
    }
}