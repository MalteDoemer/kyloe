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
        public void Test_Parsing_Expression_With_Errors(string text, params DiagnosticType[] errors)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetStatementErrorData))]
        public void Test_Parsing_Statements_With_Errors(string text, params DiagnosticType[] errors)
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

        public static IEnumerable<object[]> GetExpressionErrorData()
        {
            yield return new object[] {
                "arr[1)]",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "()",
                DiagnosticType.ExpectedExpressionError,
            };

            yield return new object[] {
                "((1)",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "(1))",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "1 + ",
                DiagnosticType.ExpectedExpressionError
            };

            yield return new object[] {
                "x += ",
                DiagnosticType.ExpectedExpressionError
            };

            yield return new object[] {
                "(1 ",
                DiagnosticType.UnexpectedTokenError
            };

            yield return new object[] {
                "hello(1, 2",
                DiagnosticType.UnexpectedTokenError
            };

            yield return new object[] {
                "data[36 + 1",
                DiagnosticType.UnexpectedTokenError
            };

            yield return new object[] {
                "x.y.",
                DiagnosticType.UnexpectedTokenError
            };
        }

        public static IEnumerable<object[]> GetStatementErrorData()
        {
            yield return new object[] {
                "1",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "(1",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "a[];",
                DiagnosticType.ExpectedExpressionError,
            };

            yield return new object[] {
                "()",
                DiagnosticType.ExpectedExpressionError,
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "var ",
                DiagnosticType.UnexpectedTokenError,
            };

            yield return new object[] {
                "var x = ;",
                DiagnosticType.ExpectedExpressionError,
            };

            
        }

        public static IEnumerable<object[]> GetExpressionTreeData()
        {
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
                    VerifyNode.NameExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                )
            };

            yield return new object[] {
                "x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.NameExpression()
                )
            };

            yield return new object[] {
                "arr[5].x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.MemberAccessExpression(
                        VerifyNode.SubscriptExpression(
                            VerifyNode.NameExpression(),
                            VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral)
                        )
                    )
                )
            };

            yield return new object[] {
                "hello()",
                VerifyNode.CallExpression(
                    VerifyNode.NameExpression()
                )
            };

            yield return new object[] {
                "hello(1, 'hello')",
                VerifyNode.CallExpression(
                    VerifyNode.NameExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenType.IntLiteral),
                    VerifyNode.LiteralExpression(SyntaxTokenType.StringLiteral)
                )
            };


        }

    }
}