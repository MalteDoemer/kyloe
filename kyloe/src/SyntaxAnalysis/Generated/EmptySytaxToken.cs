using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public sealed class EmptySytaxToken : SyntaxToken
    {
        public EmptySytaxToken()
        {
        }
        
        public override SyntaxTokenKind Kind => SyntaxTokenKind.Epsilon;
        
        public override Kyloe.Utility.SourceLocation Location => default(Kyloe.Utility.SourceLocation);
        
        public override IEnumerable<SyntaxToken> Children()
        {
            return Enumerable.Empty<SyntaxToken>();
        }
    }
}
