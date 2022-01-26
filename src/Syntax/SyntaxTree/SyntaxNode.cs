using System.IO;

namespace Kyloe.Syntax
{
    abstract class SyntaxNode
    {
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