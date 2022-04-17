using System.Collections.Generic;
using System.IO;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxNodeKind Kind { get; }

        public abstract SourceLocation Location { get; }

        public abstract IEnumerable<SyntaxNodeChild> GetChildren();

        public void WriteTo(TextWriter writer)
        {
            var pretty = new TreeWriter(writer);
            pretty.Write(this);
        }
    }

    internal abstract class SyntaxExpression : SyntaxNode { }

    internal abstract class SyntaxStatement : SyntaxNode { }
}