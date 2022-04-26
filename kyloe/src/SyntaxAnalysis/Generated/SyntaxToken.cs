using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public abstract class SyntaxToken
    {
        public abstract SyntaxTokenKind Kind { get; }
        
        public abstract Kyloe.Utility.SourceLocation Location { get; }
        
        public abstract IEnumerable<SyntaxToken?> Children();
    }
}
