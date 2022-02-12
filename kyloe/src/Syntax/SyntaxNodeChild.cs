using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal class SyntaxNodeChild
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