using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe.Tests.Parsing
{

    public class ParsingTests
    {

        [Theory]
        [MemberData(nameof(GetErrorData))]
        public void Test_Expression_Parsing_With_Errors(string text, params DiagnosticType[] errors)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }


        [Theory]
        [MemberData(nameof(GetTreeData))]
        public void Test_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }

        public static IEnumerable<object[]> GetErrorData()
        {
            yield return new object[] { "1 + ", DiagnosticType.InvalidExpressionError };
            yield return new object[] { "x += ", DiagnosticType.InvalidExpressionError };
            yield return new object[] { "(1 ", DiagnosticType.UnexpectedTokenError };
            yield return new object[] { "1 + 2)", DiagnosticType.UnmatchedClosingBracket };
            yield return new object[] { "1 + 2)", DiagnosticType.UnmatchedClosingBracket }; 
            yield return new object[] { "hello(1, 2", DiagnosticType.UnexpectedTokenError };
            yield return new object[] { "data[36 + 1", DiagnosticType.UnexpectedTokenError };
            yield return new object[] { "x.y.", DiagnosticType.UnexpectedTokenError };
        }

        public static IEnumerable<object[]> GetTreeData()
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