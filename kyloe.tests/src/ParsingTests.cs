using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe.Tests.Parsing
{

    public class ParsingTests
    {

        [Theory]
        [MemberData(nameof(GetExpressionErrorData))]
        public void Test_Parsing_Expression_With_Errors(string text, params DiagnosticKind[] errors)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetStatementErrorData))]
        public void Test_Parsing_Statements_With_Errors(string text, params DiagnosticKind[] errors)
        {
            var tree = SyntaxTree.ParseStatement(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetExpressionTreeData))]
        public void Test_Expression_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }


        [Theory]
        [MemberData(nameof(GetStatementTreeData))]
        public void Test_Statement_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.ParseStatement(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }

        public static IEnumerable<object[]> GetExpressionErrorData()
        {
            yield return new object[] {
                "arr[1)]",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.ExpectedExpressionError,
            };

            yield return new object[] {
                "((1)",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "(1))",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "1 + ",
                DiagnosticKind.ExpectedExpressionError
            };

            yield return new object[] {
                "x += ",
                DiagnosticKind.ExpectedExpressionError
            };

            yield return new object[] {
                "(1 ",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "hello(1, 2",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "data[36 + 1",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "x.y.",
                DiagnosticKind.ExpectedTokenError
            };
        }

        public static IEnumerable<object[]> GetStatementErrorData()
        {
            yield return new object[] {
                "1",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "(1",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "a[];",
                DiagnosticKind.ExpectedExpressionError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.ExpectedExpressionError,
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "var ",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "var x = ;",
                DiagnosticKind.ExpectedExpressionError,
            };
        }

        public static IEnumerable<object[]> GetExpressionTreeData()
        {
            yield return new object[] {
                "-1",
                VerifyNode.UnaryExpression(
                    SyntaxTokenType.Minus,
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };

            yield return new object[] {
                "10.5 + -47.1",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenType.FloatLiteral), 
                    SyntaxTokenType.Plus, 
                    VerifyNode.UnaryExpression(
                        SyntaxTokenType.Minus,
                        VerifyNode.LiteralExpression(SyntaxTokenType.FloatLiteral)
                    )
                )
            };

            yield return new object[] {
                "1 - -1",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral), 
                    SyntaxTokenType.Minus, 
                    VerifyNode.UnaryExpression(
                        SyntaxTokenType.Minus,
                        VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                    )
                )
            };

            yield return new object[] {
                "1 + 2",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                    SyntaxTokenType.Plus,
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };

            yield return new object[] {
                "1 + 2 * 3",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                    SyntaxTokenType.Plus,
                    VerifyNode.BinaryExpression(
                        VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                        SyntaxTokenType.Star,
                        VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                    )
                )
            };


            yield return new object[] {
                "(1 + 2) * 3",
                VerifyNode.BinaryExpression(
                    VerifyNode.ParenthsizedExpression(
                        VerifyNode.BinaryExpression(
                            VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                            SyntaxTokenType.Plus,
                            VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                        )
                    ),
                    SyntaxTokenType.Star,
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };

            yield return new object[] {
                "arr[5]",
                VerifyNode.SubscriptExpression(
                    VerifyNode.IdentifierExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };

            yield return new object[] {
                "x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.IdentifierExpression()
                )
            };

            yield return new object[] {
                "arr[5].x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.MemberAccessExpression(
                        VerifyNode.SubscriptExpression(
                            VerifyNode.IdentifierExpression(),
                            VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                        )
                    )
                )
            };

            yield return new object[] {
                "hello()",
                VerifyNode.CallExpression(
                    VerifyNode.IdentifierExpression()
                )
            };

            yield return new object[] {
                "hello(1, 'hello')",
                VerifyNode.CallExpression(
                    VerifyNode.IdentifierExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                    VerifyNode.LiteralExpression(SyntaxTokenType.StringLiteral)
                )
            };

            yield return new object[] {
                "x += 3",
                VerifyNode.AssignmentExpression(
                    VerifyNode.IdentifierExpression(),
                    SyntaxTokenType.PlusEquals,
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };
        }

        public static IEnumerable<object[]> GetStatementTreeData()
        {
            yield return new object[] {
                "x += 3;",
                VerifyNode.ExpressionStatement(
                    VerifyNode.AssignmentExpression(
                        VerifyNode.IdentifierExpression(),
                        SyntaxTokenType.PlusEquals,
                        VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                    )
                )
            };

            yield return new object[] {
                "hello(1.2);",
                VerifyNode.ExpressionStatement(
                    VerifyNode.CallExpression(
                        VerifyNode.IdentifierExpression(),
                        VerifyNode.LiteralExpression(SyntaxTokenType.FloatLiteral)
                    )
                )
            };

            yield return new object[] {
                ";",
                VerifyNode.EmptyStatement()
            };
        }
    }
}