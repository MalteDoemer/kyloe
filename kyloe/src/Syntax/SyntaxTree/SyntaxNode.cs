using System.IO;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    abstract class SyntaxNode
    {
        public abstract SourceLocation Location { get; }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                var prettyWriter = new PrettyWriter(writer);
                prettyWriter.Write(this);
                return writer.ToString();
            }
        }
    }
}