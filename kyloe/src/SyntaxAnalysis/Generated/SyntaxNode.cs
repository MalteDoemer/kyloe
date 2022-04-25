using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public sealed class SyntaxNode : SyntaxToken
    {
        public SyntaxNode(SyntaxTokenKind kind, ImmutableArray<SyntaxToken?> tokens)
        {
            Kind = kind;
            Tokens = tokens;
        }
        
        public override SyntaxTokenKind Kind { get; }
        
        public ImmutableArray<SyntaxToken?> Tokens { get; }
        
        public override Kyloe.Utility.SourceLocation Location => Kyloe.Utility.SourceLocation.CreateAround(Children().First().Location, Children().Last().Location);
        
        public override IEnumerable<SyntaxToken> Children()
        {
            return Tokens.Where(t => t is not null)!;
        }
    }
}
