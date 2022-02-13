using System.IO;

namespace Kyloe.Syntax
{
    internal class TreeWriter
    {
        private const string CHILD_INDENT = "│   ";
        private const string LEAF_INDENT = "    ";
        private const string CHILD_NODE = "├── ";
        private const string LEAF_NODE = "└── ";

        private enum ChildType
        {
            Root,
            Leaf,
            Normal,
        }

        private readonly TextWriter writer;

        public TreeWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(SyntaxNode root)
        {
            var child = new SyntaxNodeChild(root);
            WriteChild(child, "", ChildType.Root);
        }

        private void WriteChild(SyntaxNodeChild node, string indent, ChildType thisType)
        {
            writer.Write(indent);

            if (thisType == ChildType.Normal)
                writer.Write(CHILD_NODE);
            else if (thisType == ChildType.Leaf)
                writer.Write(LEAF_NODE);

            writer.WriteLine(node.ToString());

            var children = node.GetChildren().GetEnumerator();

            if (children is null)
                return;

            // initially the IEnumerator points to the element before the first one
            // so MoveNext() has to be called before accessing Current
            if (!children.MoveNext())
                return;

            while (true)
            {
                var next = children.Current;
                bool hasNext = children.MoveNext();

                string nextIndent = "";

                if (thisType == ChildType.Normal)
                    nextIndent = indent + CHILD_INDENT;
                else if (thisType == ChildType.Leaf)
                    nextIndent = indent + LEAF_INDENT;

                WriteChild(next, nextIndent, hasNext ? ChildType.Normal : ChildType.Leaf);

                if (!hasNext)
                    break;
            }

        }       
    }
}