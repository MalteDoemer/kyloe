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



            if (nodes.Length == 0)
                throw new System.ArgumentException("must have at least one element", nameof(nodes));
        }

        public ImmutableArray<SyntaxNode> Nodes { get; }
        public ImmutableArray<SyntaxToken> Commas { get; }

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