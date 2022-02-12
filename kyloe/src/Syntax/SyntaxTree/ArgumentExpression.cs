using System.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class ArgumentExpression : SyntaxNode
    {
        public ArgumentExpression(ImmutableArray<SyntaxNode> nodes, ImmutableArray<SyntaxToken> commas)
        {
            this.Nodes = nodes;
            this.Commas = commas;

            Debug.Assert(nodes.Length != 0, "nodes must have at least one element");
        }

        public ImmutableArray<SyntaxNode> Nodes { get; }
        public ImmutableArray<SyntaxToken> Commas { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ArgumentExpression;

        public override SourceLocation Location
        {
            get
            {
                if (Commas.Length < Nodes.Length)
                    return SourceLocation.CreateAround(Nodes.First().Location, Nodes.Last().Location);
                else
                    return SourceLocation.CreateAround(Nodes.First().Location, Commas.Last().Location);
            }
        }

    }
}