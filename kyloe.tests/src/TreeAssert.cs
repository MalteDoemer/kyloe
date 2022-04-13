using System;
using System.Linq;
using System.Diagnostics;
using Xunit;
using Kyloe.Syntax;
using System.Collections.Generic;

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

        public static VerifyNode IdentifierExpression()
        {
            return new VerifyNode(SyntaxNodeType.IdentifierExpression, new VerifyNode(SyntaxTokenType.Identifier));
        }

        public static VerifyNode SubscriptExpression(VerifyNode expr, VerifyNode index)
        {
            return new VerifyNode(SyntaxNodeType.SubscriptExpression, expr, new VerifyNode(SyntaxTokenType.LeftSquare), index, new VerifyNode(SyntaxTokenType.RightSquare));
        }

        public static VerifyNode MemberAccessExpression(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.MemberAccessExpression, expr, new VerifyNode(SyntaxTokenType.Dot), IdentifierExpression());
        }

        public static VerifyNode CallExpression(VerifyNode expr, params VerifyNode[] args)
        {
            if (args.Length == 0)
                return new VerifyNode(SyntaxNodeType.CallExpression, expr, new VerifyNode(SyntaxTokenType.LeftParen), new VerifyNode(SyntaxTokenType.RightParen));

            var children = new List<VerifyNode>();
            children.Add(expr);
            children.Add(new VerifyNode(SyntaxTokenType.LeftParen));

            for (int i = 0; i < args.Length - 1; i++)
            {
                children.Add(args[i]);
                children.Add(new VerifyNode(SyntaxTokenType.Comma));
            }

            children.Add(args.Last());
            children.Add(new VerifyNode(SyntaxTokenType.RightParen));

            return new VerifyNode(SyntaxNodeType.CallExpression, children.ToArray());
        }

        public static VerifyNode AssignmentExpression(VerifyNode left, SyntaxTokenType op, VerifyNode right)
        {
            return new VerifyNode(SyntaxNodeType.AssignmentExpression, left, new VerifyNode(op), right);
        }

        public static VerifyNode ExpressionStatement(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeType.ExpressionStatement, expr, new VerifyNode(SyntaxTokenType.SemiColon));
        }

        public static VerifyNode DeclarationStatement(SyntaxTokenType declToken, VerifyNode expr)
        {
            return new VerifyNode(
                SyntaxNodeType.DeclarationStatement,
                new VerifyNode(declToken),
                new VerifyNode(SyntaxTokenType.Identifier),
                new VerifyNode(SyntaxTokenType.Equals),
                expr,
                new VerifyNode(SyntaxTokenType.SemiColon)
            );
        }

        public static VerifyNode IfStatement(VerifyNode condition, VerifyNode body)
        {
            return new VerifyNode(SyntaxNodeType.IfStatement, new VerifyNode(SyntaxTokenType.IfKeyword), condition, body);
        }

        public static VerifyNode IfElseStatement(VerifyNode condition, VerifyNode body, VerifyNode elseBody)
        {
            return new VerifyNode(
                SyntaxNodeType.IfStatement,
                new VerifyNode(SyntaxTokenType.IfKeyword),
                condition,
                body,
                new VerifyNode(SyntaxTokenType.ElseKeyword),
                elseBody
            );
        }

        public static VerifyNode EmptyStatement()
        {
            return new VerifyNode(SyntaxNodeType.EmptyStatement, new VerifyNode(SyntaxTokenType.SemiColon));
        }

        public static VerifyNode BlockStatement(params VerifyNode[] statements)
        {
            var nodes = new List<VerifyNode>(statements.Length + 2);
            nodes.Add(new VerifyNode(SyntaxTokenType.LeftCurly));
            foreach (var stmt in statements)
                nodes.Add(stmt);
            nodes.Add(new VerifyNode(SyntaxTokenType.RightCurly));

            return new VerifyNode(SyntaxNodeType.BlockStatement, nodes.ToArray());
        }

        public static VerifyNode SimpleParameterDeclaration()
        {
            return new VerifyNode(
                SyntaxNodeType.ParameterDeclaration,
                new VerifyNode(SyntaxTokenType.Identifier),
                new VerifyNode(SyntaxTokenType.Colon),
                VerifyNode.IdentifierExpression()
            );
        }

        public static VerifyNode FunctionDefinition(int numParameters, VerifyNode body, bool typeClause)
        {
            var nodes = new List<VerifyNode>(numParameters + 5);

            nodes.Add(new VerifyNode(SyntaxTokenType.FuncKeyword));
            nodes.Add(new VerifyNode(SyntaxTokenType.Identifier));
            nodes.Add(new VerifyNode(SyntaxTokenType.LeftParen));

            for (int i = 0; i < numParameters; i++)
            {
                nodes.Add(SimpleParameterDeclaration());

                if (i != numParameters - 1)
                    nodes.Add(new VerifyNode(SyntaxTokenType.Comma));
            }

            nodes.Add(new VerifyNode(SyntaxTokenType.RightParen));

            if (typeClause)
            {
                nodes.Add(new VerifyNode(SyntaxTokenType.SmallArrow));
                nodes.Add(IdentifierExpression());
            }

            nodes.Add(body);

            return new VerifyNode(
                SyntaxNodeType.FunctionDefinition,
                nodes.ToArray()
            );
        }

        public static VerifyNode CompilationUnitSyntax(params VerifyNode[] nodes)
        {
            return new VerifyNode(SyntaxNodeType.CompilationUnitSyntax, nodes);
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