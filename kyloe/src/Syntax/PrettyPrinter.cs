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
            switch (node.Type)
            {
                case SyntaxNodeType.MalformedExpression:
                    var malformedExpression = (MalformedExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(MalformedExpression)}: {malformedExpression.Token}");
                    break;
                case SyntaxNodeType.LiteralExpression:
                    var literalExpression = (LiteralExpression)node;
                    writer.Write(indent);
                    writer.WriteLine(literalExpression.LiteralToken);
                    break;
                case SyntaxNodeType.UnaryExpression:
                    var unaryExpression = (UnaryExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(UnaryExpression)}: {unaryExpression.OperatorToken}");
                    Write(unaryExpression.Child, indent + INCREMENT);
                    break;
                case SyntaxNodeType.BinaryExpression:
                    var binaryExpression = (BinaryExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(BinaryExpression)}: {binaryExpression.OperatorToken}");
                    Write(binaryExpression.LeftChild, indent + INCREMENT);
                    Write(binaryExpression.RightChild, indent + INCREMENT);
                    break;
                case SyntaxNodeType.ParenthesizedExpression:
                    var parenthesizedExpression = (ParenthesizedExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(ParenthesizedExpression)}: ");
                    Write(parenthesizedExpression.Expression, indent + INCREMENT);
                    break;
                case SyntaxNodeType.NameExpression:
                    var nameExpression = (NameExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(NameExpression)}: ");
                    writer.Write(indent + INCREMENT);
                    writer.WriteLine(nameExpression.NameToken);
                    break;
                case SyntaxNodeType.MemberAccessExpression:
                    var memberAccessExpression = (MemberAccessExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(MemberAccessExpression)}: ");
                    Write(memberAccessExpression.Expression, indent + INCREMENT);
                    writer.Write(indent + INCREMENT);
                    writer.WriteLine(memberAccessExpression.NameToken);
                    break;
                case SyntaxNodeType.SubscriptExpression:
                    var subscriptExpression = (SubscriptExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(SubscriptExpression)}: ");
                    Write(subscriptExpression.LeftNode, indent + INCREMENT);
                    Write(subscriptExpression.Subscript, indent + INCREMENT);
                    break;
                case SyntaxNodeType.CallExpression:
                    var callExpression = (CallExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(CallExpression)}: ");
                    Write(callExpression.Expression, indent + INCREMENT);

                    if (callExpression.Arguments is not null)
                    {
                        Write(callExpression.Arguments, indent + INCREMENT);
                    }
                    break;
                case SyntaxNodeType.ArgumentExpression:
                    var argumentExpression = (ArgumentExpression)node;
                    writer.Write(indent);
                    writer.WriteLine($"{nameof(ArgumentExpression)}: ");

                    var nextIndent = indent + INCREMENT;

                    foreach (var child in argumentExpression.Nodes)
                    {
                        Write(child, nextIndent);
                    }
                    break;
                default:
                    throw new System.Exception($"Unknown node: {node.Type}");
            }
        }
    }
}