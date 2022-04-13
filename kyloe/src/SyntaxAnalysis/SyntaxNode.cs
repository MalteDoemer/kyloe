using System.Collections.Generic;
using System.IO;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxNodeType Type { get; }

        public abstract SourceLocation Location { get; }

        public abstract IEnumerable<SyntaxNodeChild> GetChildren();

        public void WriteTo(TextWriter writer)
        {
            var pretty = new TreeWriter(writer);
            pretty.Write(this);
        }
    }
}