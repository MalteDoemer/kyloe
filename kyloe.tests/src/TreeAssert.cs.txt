using System;
using System.Linq;
using System.Diagnostics;
using Xunit;
using Kyloe.Syntax;
using System.Collections.Generic;

namespace Kyloe.Tests
{
    public struct SyntaxNodeOrTokenKind
    {
        private readonly SyntaxTokenKind tokenKind;
        private readonly SyntaxNodeKind nodeKind;
        private readonly bool isNode;

        public SyntaxNodeOrTokenKind(SyntaxNodeKind kind)
        {
            isNode = true;
            nodeKind = kind;
            tokenKind = default(SyntaxTokenKind);
        }

        public SyntaxNodeOrTokenKind(SyntaxTokenKind kind)
        {
            isNode = false;
            tokenKind = kind;
            nodeKind = default(SyntaxNodeKind);
        }

        public bool IsNode => isNode;
        public bool IsToken => !isNode;

        public SyntaxTokenKind TokenKind
        {
            get
            {
                Debug.Assert(IsToken);
                return tokenKind;
            }
        }

        public SyntaxNodeKind NodeKind
        {
            get
            {
                Debug.Assert(IsNode);
                return nodeKind;
            }
        }

    }

    public class VerifyNode
    {
        public VerifyNode(SyntaxNodeKind expected, params VerifyNode[] children)
        {
            Kind = new SyntaxNodeOrTokenKind(expected);
            Children = children;
        }

        public VerifyNode(SyntaxTokenKind expected)
        {
            Kind = new SyntaxNodeOrTokenKind(expected);
            Children = Array.Empty<VerifyNode>();
        }

        public SyntaxNodeOrTokenKind Kind { get; }
        public VerifyNode[] Children { get; }

        public bool IsNode => Kind.IsNode;
        public bool IsToken => Kind.IsToken;

        public static VerifyNode LiteralExpression(SyntaxTokenKind kind)
        {
            return new VerifyNode(SyntaxNodeKind.LiteralExpression, new VerifyNode(kind));
        }

        public static VerifyNode BinaryExpression(VerifyNode left, SyntaxTokenKind op, VerifyNode right)
        {
            return new VerifyNode(SyntaxNodeKind.BinaryExpression, left, new VerifyNode(op), right);
        }

        public static VerifyNode UnaryExpression(SyntaxTokenKind op, VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeKind.UnaryExpression, new VerifyNode(op), expr);
        }

        public static VerifyNode ParenthsizedExpression(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeKind.ParenthesizedExpression, new VerifyNode(SyntaxTokenKind.LeftParen), expr, new VerifyNode(SyntaxTokenKind.RightParen));
        }

        public static VerifyNode IdentifierExpression()
        {
            return new VerifyNode(SyntaxNodeKind.IdentifierExpression, new VerifyNode(SyntaxTokenKind.Identifier));
        }

        public static VerifyNode SubscriptExpression(VerifyNode expr, VerifyNode index)
        {
            return new VerifyNode(SyntaxNodeKind.SubscriptExpression, expr, new VerifyNode(SyntaxTokenKind.LeftSquare), index, new VerifyNode(SyntaxTokenKind.RightSquare));
        }

        public static VerifyNode MemberAccessExpression(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeKind.MemberAccessExpression, expr, new VerifyNode(SyntaxTokenKind.Dot), IdentifierExpression());
        }

        public static VerifyNode CallExpression(VerifyNode expr, params VerifyNode[] args)
        {
            if (args.Length == 0)
                return new VerifyNode(SyntaxNodeKind.CallExpression, expr, new VerifyNode(SyntaxTokenKind.LeftParen), new VerifyNode(SyntaxTokenKind.RightParen));

            var children = new List<VerifyNode>();
            children.Add(expr);
            children.Add(new VerifyNode(SyntaxTokenKind.LeftParen));

            for (int i = 0; i < args.Length - 1; i++)
            {
                children.Add(args[i]);
                children.Add(new VerifyNode(SyntaxTokenKind.Comma));
            }

            children.Add(args.Last());
            children.Add(new VerifyNode(SyntaxTokenKind.RightParen));

            return new VerifyNode(SyntaxNodeKind.CallExpression, children.ToArray());
        }

        public static VerifyNode AssignmentExpression(VerifyNode left, SyntaxTokenKind op, VerifyNode right)
        {
            return new VerifyNode(SyntaxNodeKind.AssignmentExpression, left, new VerifyNode(op), right);
        }

        public static VerifyNode ExpressionStatement(VerifyNode expr)
        {
            return new VerifyNode(SyntaxNodeKind.ExpressionStatement, expr, new VerifyNode(SyntaxTokenKind.SemiColon));
        }

        public static VerifyNode DeclarationStatement(SyntaxTokenKind declToken, VerifyNode expr)
        {
            return new VerifyNode(
                SyntaxNodeKind.DeclarationStatement,
                new VerifyNode(declToken),
                new VerifyNode(SyntaxTokenKind.Identifier),
                new VerifyNode(SyntaxTokenKind.Equals),
                expr,
                new VerifyNode(SyntaxTokenKind.SemiColon)
            );
        }

        public static VerifyNode IfStatement(VerifyNode condition, VerifyNode body)
        {
            return new VerifyNode(SyntaxNodeKind.IfStatement, new VerifyNode(SyntaxTokenKind.IfKeyword), condition, body);
        }

        public static VerifyNode IfElseStatement(VerifyNode condition, VerifyNode body, VerifyNode elseBody)
        {
            return new VerifyNode(
                SyntaxNodeKind.IfStatement,
                new VerifyNode(SyntaxTokenKind.IfKeyword),
                condition,
                body,
                new VerifyNode(SyntaxTokenKind.ElseKeyword),
                elseBody
            );
        }

        public static VerifyNode EmptyStatement()
        {
            return new VerifyNode(SyntaxNodeKind.EmptyStatement, new VerifyNode(SyntaxTokenKind.SemiColon));
        }

        public static VerifyNode BlockStatement(params VerifyNode[] statements)
        {
            var nodes = new List<VerifyNode>(statements.Length + 2);
            nodes.Add(new VerifyNode(SyntaxTokenKind.LeftCurly));
            foreach (var stmt in statements)
                nodes.Add(stmt);
            nodes.Add(new VerifyNode(SyntaxTokenKind.RightCurly));

            return new VerifyNode(SyntaxNodeKind.BlockStatement, nodes.ToArray());
        }

        public static VerifyNode SimpleParameterDeclaration()
        {
            return new VerifyNode(
                SyntaxNodeKind.ParameterDeclaration,
                new VerifyNode(SyntaxTokenKind.Identifier),
                new VerifyNode(SyntaxTokenKind.Colon),
                VerifyNode.IdentifierExpression()
            );
        }

        public static VerifyNode FunctionDefinition(int numParameters, VerifyNode body, bool typeClause)
        {
            var nodes = new List<VerifyNode>(numParameters + 5);

            nodes.Add(new VerifyNode(SyntaxTokenKind.FuncKeyword));
            nodes.Add(new VerifyNode(SyntaxTokenKind.Identifier));
            nodes.Add(new VerifyNode(SyntaxTokenKind.LeftParen));

            for (int i = 0; i < numParameters; i++)
            {
                nodes.Add(SimpleParameterDeclaration());

                if (i != numParameters - 1)
                    nodes.Add(new VerifyNode(SyntaxTokenKind.Comma));
            }

            nodes.Add(new VerifyNode(SyntaxTokenKind.RightParen));

            if (typeClause)
            {
                nodes.Add(new VerifyNode(SyntaxTokenKind.SmallArrow));
                nodes.Add(IdentifierExpression());
            }

            nodes.Add(body);

            return new VerifyNode(
                SyntaxNodeKind.FunctionDefinition,
                nodes.ToArray()
            );
        }

        public static VerifyNode CompilationUnitSyntax(params VerifyNode[] nodes)
        {
            return new VerifyNode(SyntaxNodeKind.CompilationUnitSyntax, nodes);
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
                Assert.Equal(verify.Kind.NodeKind, node.Node!.Kind);

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
                Assert.Equal(verify.Kind.TokenKind, node.Token!.Kind);
            }
        }

    }
}