using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxNodeType Type { get; }

        public abstract SourceLocation Location { get; }

        public abstract IEnumerable<SyntaxNodeChild> GetChildren();
    }
}