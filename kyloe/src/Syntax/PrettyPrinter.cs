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
                writer.WriteLine($"{nameof(MalformedSyntaxNode)}: {malformedSyntaxNode.Token}");
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
            else if (node is ParenthesizedExpressionNode parenthesizedExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(ParenthesizedExpressionNode)}: ");
                Write(parenthesizedExpressionNode.Expression, indent + INCREMENT);
            }
            else if (node is NameExpressionNode nameExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(NameExpressionNode)}: ");
                writer.Write(indent + INCREMENT);
                writer.WriteLine(nameExpressionNode.NameToken);
            }
            else if (node is MemberAccessNode memberAccessNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(MemberAccessNode)}: ");
                Write(memberAccessNode.Expression, indent + INCREMENT);
                writer.Write(indent + INCREMENT);
                writer.WriteLine(memberAccessNode.NameToken);
            } else if (node is SubscriptExpressionNode subscriptExpressionNode) {
                writer.Write(indent);
                writer.WriteLine($"{nameof(SubscriptExpressionNode)}: ");
                Write(subscriptExpressionNode.LeftNode, indent + INCREMENT);
                Write(subscriptExpressionNode.Subscript, indent + INCREMENT);
            }
            else
            {
                throw new System.Exception($"Unknown node: {node.GetType()}");
            }
        }
    }
}