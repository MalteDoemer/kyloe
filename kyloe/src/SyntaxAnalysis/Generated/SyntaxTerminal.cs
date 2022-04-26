using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public sealed class SyntaxTerminal : SyntaxToken
    {
        public SyntaxTerminal(SyntaxTokenKind kind, string text, Kyloe.Utility.SourceLocation location, bool invalid = false)
        {
            Kind = kind;
            Text = text;
            Location = location;
            Invalid = invalid;
        }
        
        public override SyntaxTokenKind Kind { get; }
        
        public string Text { get; }
        
        public override Kyloe.Utility.SourceLocation Location { get; }
        
        public bool Invalid { get; }
        
        public override IEnumerable<SyntaxToken?> Children()
        {
            return Enumerable.Empty<SyntaxToken>();
        }
    }
}
