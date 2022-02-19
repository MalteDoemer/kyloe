using System;
using System.Linq;
using System.Diagnostics;
using Xunit;
using Kyloe.Syntax;

namespace Kyloe.Tests
{
    public struct SyntaxNodeOrTokenType
    {
        private readonly SyntaxTokenType tokenType;
        private readonly SyntaxNodeType nodeType;
        private readonly bool isNodeType;

        public SyntaxNodeOrTokenType(SyntaxNodeType type)
        {
            isNodeType = true;
            nodeType = type;
            tokenType = default(SyntaxTokenType);
        }

        public SyntaxNodeOrTokenType(SyntaxTokenType type)
        {
            isNodeType = false;
            tokenType = type;
            nodeType = default(SyntaxNodeType);
        }

        public bool IsNode => isNodeType;
        public bool IsToken => !isNodeType;

        public SyntaxTokenType TokenType
        {
            get
            {
                Debug.Assert(IsToken);
                return tokenType;
            }
        }

        public SyntaxNodeType NodeType
        {
            get
            {
                Debug.Assert(IsNode);
                return nodeType;
            }
        }

    }

    public class VerifyNode
    {
        public VerifyNode(SyntaxNodeType expected, params VerifyNode[] children)
        {
            Type = new SyntaxNodeOrTokenType(expected);
            Children = children;
        }

        public VerifyNode(SyntaxTokenType expected)
        {
            Type = new SyntaxNodeOrTokenType(expected);
            Children = Array.Empty<VerifyNode>();
        }

        public SyntaxNodeOrTokenType Type { get; }
        public VerifyNode[] Children { get; }

        public bool IsNode => Type.IsNode;
        public bool IsToken => Type.IsToken;

        public static VerifyNode LiteralExpression(SyntaxTokenType type)
        {
            return new VerifyNode(SyntaxNodeType.LiteralExpression, new VerifyNode(type));
        }

        public static VerifyNode BinaryExpression(VerifyNode left, SyntaxTokenType op, VerifyNode right)
        {
            return new VerifyNode(SyntaxNodeType.BinaryExpression, left, new VerifyNode(op), right);
        }

        public static VerifyNode UnaryExpression(SyntaxTokenType op, VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.UnaryExpression, new VerifyNode(op), expr);
        }

        public static VerifyNode ParenthsizedExpression(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.ParenthesizedExpression, new VerifyNode(SyntaxTokenType.LeftParen), expr, new VerifyNode(SyntaxTokenType.RightParen));
        }

        public static VerifyNode NameExpression()
        {
            return new VerifyNode(SyntaxNodeType.NameExpression, new VerifyNode(SyntaxTokenType.Identifier));
        }

        public static VerifyNode SubscriptExpression(VerifyNode expr, VerifyNode index)
        {
            return new VerifyNode(SyntaxNodeType.SubscriptExpression, expr, new VerifyNode(SyntaxTokenType.LeftSquare), index, new VerifyNode(SyntaxTokenType.RightSquare));
        }

        public static VerifyNode MemberAccessExpression(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.MemberAccessExpression, expr, new VerifyNode(SyntaxTokenType.Dot), NameExpression());
        }

        public static VerifyNode CallExpression(VerifyNode epxr, params VerifyNode[] args)
        {
            if (args.Length == 0)
                return new VerifyNode(SyntaxNodeType.CallExpression, epxr, new VerifyNode(SyntaxTokenType.LeftParen), new VerifyNode(SyntaxTokenType.RightParen));

            var argsAndCommas = new VerifyNode[args.Length * 2 - 1];

            for (int i = 0; i < args.Length; i++)
            {
                argsAndCommas[i * 2] = args[i];
                if (i != args.Length - 1)
                {
                    argsAndCommas[i * 2 + 1] = new VerifyNode(SyntaxTokenType.Comma);
                }
            }

            var argNode = new VerifyNode(SyntaxNodeType.ArgumentExpression, argsAndCommas);


            return new VerifyNode(SyntaxNodeType.CallExpression, epxr, new VerifyNode(SyntaxTokenType.LeftParen), argNode, new VerifyNode(SyntaxTokenType.RightParen));
        }

        public static VerifyNode AssignmentExpression(VerifyNode left, SyntaxTokenType op, VerifyNode right)
        {
            return new VerifyNode(SyntaxNodeType.AssignmentExpression, left, new VerifyNode(op), right);
        }

        public static VerifyNode EmptyStatement()
        {
            return new VerifyNode(SyntaxNodeType.EmptyStatement, new VerifyNode(SyntaxTokenType.SemiColon));
        }

        public static VerifyNode ExpressionStatement(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.ExpressionStatement, expr, new VerifyNode(SyntaxTokenType.SemiColon));
        }
    }

    internal class TreeAssert
    {
        public static void Verify(SyntaxTree tree, VerifyNode verify)
        {
            Verify(new SyntaxNodeChild(tree.GetRoot()), verify);
        }

        public static void Verify(SyntaxNodeChild node, VerifyNode verify)
        {
            Assert.Equal(node.IsNode, verify.IsNode);

            if (node.IsNode)
            {
                Assert.Equal(verify.Type.NodeType, node.Node!.Type);

                Assert.Equal(verify.Children.Length, node.GetChildren().Count());

                foreach (var elem in node.GetChildren().Select((Node, Index) => new { Index, Node }))
                {
                    var nodeChild = elem.Node;
                    var verifyChild = verify.Children[elem.Index];
                    Verify(nodeChild, verifyChild);
                }
            }
            else
            {
                Assert.Equal(verify.Type.TokenType, node.Token!.Type);
            }
        }

    }
}