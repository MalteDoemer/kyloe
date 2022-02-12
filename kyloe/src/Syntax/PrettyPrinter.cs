using System.IO;

namespace Kyloe.Syntax
{
    public class PrettyWriter
    {
        private const string INCREMENT = "    ";

        private TextWriter writer;

        public PrettyWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(SyntaxNode node, string indent = "")
        {
            if (node is LiteralExpression literalSyntaxNode)
            {
                writer.Write(indent);
                writer.WriteLine(literalSyntaxNode.LiteralToken);
            }
            else if (node is MalformedExpression malformedSyntaxNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(MalformedExpression)}: {malformedSyntaxNode.Token}");
            }
            else if (node is UnaryExpression unaryExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(UnaryExpression)}: {unaryExpressionNode.OperatorToken}");
                Write(unaryExpressionNode.Child, indent + INCREMENT);
            }
            else if (node is BinaryExpression binaryExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(BinaryExpression)}: {binaryExpressionNode.OperatorToken}");
                Write(binaryExpressionNode.LeftChild, indent + INCREMENT);
                Write(binaryExpressionNode.RightChild, indent + INCREMENT);
            }
            else if (node is ParenthesizedExpression parenthesizedExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(ParenthesizedExpression)}: ");
                Write(parenthesizedExpressionNode.Expression, indent + INCREMENT);
            }
            else if (node is NameExpression nameExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(NameExpression)}: ");
                writer.Write(indent + INCREMENT);
                writer.WriteLine(nameExpressionNode.NameToken);
            }
            else if (node is MemberAccessExpression memberAccessNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(MemberAccessExpression)}: ");
                Write(memberAccessNode.Expression, indent + INCREMENT);
                writer.Write(indent + INCREMENT);
                writer.WriteLine(memberAccessNode.NameToken);
            }
            else if (node is SubscriptExpression subscriptExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(SubscriptExpression)}: ");
                Write(subscriptExpressionNode.LeftNode, indent + INCREMENT);
                Write(subscriptExpressionNode.Subscript, indent + INCREMENT);
            }
            else if (node is CallExpression callExpressionNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(CallExpression)}: ");
                Write(callExpressionNode.Expression, indent + INCREMENT);

                if (callExpressionNode.Arguments is not null)
                {
                    Write(callExpressionNode.Arguments, indent + INCREMENT);
                }
            }
            else if (node is ArgumentExpression argumentNode)
            {
                writer.Write(indent);
                writer.WriteLine($"{nameof(ArgumentExpression)}: ");

                var nextIndent = indent + INCREMENT;

                foreach (var child in argumentNode.Nodes)
                {
                    Write(child, nextIndent);
                }
            }
            else
            {
                throw new System.Exception($"Unknown node: {node.GetType()}");
            }
        }
    }
}