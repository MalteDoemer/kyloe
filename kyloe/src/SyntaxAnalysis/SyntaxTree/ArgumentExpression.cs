using System.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Kyloe.Utility;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal class ArgumentExpression: SyntaxExpression
    {
        public ArgumentExpression(ImmutableArray<SyntaxNode> nodes, ImmutableArray<SyntaxToken> commas)
        {
            Nodes = nodes;
            Commas = commas;

            Debug.Assert(nodes.Length != 0, "nodes must have at least one element");
            Debug.Assert(nodes.Length == Commas.Length + 1, "there must be one node more than commas");
        }

        public ImmutableArray<SyntaxNode> Nodes { get; }
        public ImmutableArray<SyntaxToken> Commas { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ArgumentExpression;

        public override SourceLocation Location => SourceLocation.CreateAround(Nodes.First().Location, Nodes.Last().Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            for (int i = 0; i < Commas.Length; i++)
            {
                yield return new SyntaxNodeChild(Nodes[i]);
                yield return new SyntaxNodeChild(Commas[i]);
            }

            yield return new SyntaxNodeChild(Nodes.Last());
        }
    }
}