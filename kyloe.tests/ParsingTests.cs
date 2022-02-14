using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe.Tests.Parsing
{

    public class ParsingTests
    {

        [Theory]
        [InlineData("1 + ", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        [InlineData("x += ", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        [InlineData("(1 ", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        [InlineData("hello(1, 2", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        [InlineData("data[36 + 1", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        [InlineData("x.y.", new DiagnosticType[] { DiagnosticType.UnexpectedTokenError })]
        public void Test_Parsing_With_Errors(string text, DiagnosticType[] errors) 
        {
            var tree = SyntaxTree.Parse(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }


        [Theory]
        [MemberData(nameof(GetTreeData))]
        public void Test_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.Parse(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
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