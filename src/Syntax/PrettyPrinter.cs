using System.IO;

namespace Kyloe.Syntax
{
    class PrettyWriter
    {
        private const string INCREMENT = "    ";

        private TextWriter writer;

        public PrettyWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(SyntaxNode node, string indent = "")
        {
            if (node is LiteralSyntaxNode literalSyntaxNode)
            {
                writer.Write(indent);
                writer.WriteLine(literalSyntaxNode.LiteralToken);
            }
            else if (node is MalformedSyntaxNode malformedSyntaxNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(MalformedSyntaxNode)}: ${malformedSyntaxNode.Token}");
            }
            else if (node is UnaryExpressionNode unaryExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(UnaryExpressionNode)}: {unaryExpressionNode.OperatorToken}");
                Write(unaryExpressionNode.Child, indent + INCREMENT);
            }
            else if (node is BinaryExpressionNode binaryExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(BinaryExpressionNode)}: {binaryExpressionNode.OperatorToken}");
                Write(binaryExpressionNode.LeftChild, indent + INCREMENT);
                Write(binaryExpressionNode.RightChild, indent + INCREMENT);
            }
        }
    }
}