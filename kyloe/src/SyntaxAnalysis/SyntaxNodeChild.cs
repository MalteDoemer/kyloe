using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public class SyntaxNodeChild
    {
        private readonly SyntaxNode? node;
        private readonly SyntaxToken? token;


        public SyntaxNodeChild(SyntaxNode node)
        {
            this.node = node;
        }

        public SyntaxNodeChild(SyntaxToken token)
        {
            this.token = token;
        }

        public SyntaxNode? Node => node;

        public SyntaxToken? Token => token;

        public bool IsNode => Node is not null;
        public bool IsToken => Token is not null;

        public override string ToString()
        {
            if (node is not null)
                return node.GetType().Name;
            else
                return token!.ToString();
        }

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            if (node is not null)
                foreach (var child in node.GetChildren())
                    yield return child;
            else
                yield break;
        }
    }
}