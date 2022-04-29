using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe.Tests.Parsing
{
    public class ParsingTests
    {

        private SyntaxTree ParseExpression(string text)
        {
            var modified = $"var _ = {text};";
            return SyntaxTree.Parse(modified);
        }

        private SyntaxTree ParseStatement(string text)
        {
            var modified = $"func main() {{ {text} }}";
            return SyntaxTree.Parse(modified);
        }

        [Theory]
        [MemberData(nameof(GetExpressionData))]
        public void Test_Parsing_Expression(string text, params DiagnosticKind[] errors)
        {
            var tree = ParseExpression(text);
            DiagnosticAssert.Equals(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetStatementData))]
        public void Test_Parsing_Statements(string text, params DiagnosticKind[] errors)
        {
            var tree = ParseStatement(text);
            DiagnosticAssert.Equals(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetWholeProgramData))]
        public void Test_Parsing_Program(string text, params DiagnosticKind[] errors)
        {
            var tree = SyntaxTree.Parse(text);
            DiagnosticAssert.Equals(tree.GetDiagnostics(), errors);
        }

        public static IEnumerable<object[]> GetExpressionData()
        {
            yield return new object[] {
                "(1 + 2)",
            };

            yield return new object[] {
                "hello()",
            };

            yield return new object[] {
                "x.y.z(1, 2, 3)",
            };

            yield return new object[] {
                "array[5][6]('test')",
            };

            yield return new object[] {
                "true || false && x | y ^ z & y != u < v + 5 * 8",
            };

            yield return new object[] {
                "x = y = z += 5 -= 23",
            };

            yield return new object[] {
                "° hi §",
                DiagnosticKind.InvalidCharacterError,
                DiagnosticKind.InvalidCharacterError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "var",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "println('hi', )",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "((1)",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "(1))",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "a[]",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "1 + ",
                DiagnosticKind.UnexpectedTokenError
            };

            yield return new object[] {
                "2 * * ",
                DiagnosticKind.UnexpectedTokenError,
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "x += ",
                DiagnosticKind.UnexpectedTokenError
            };

            yield return new object[] {
                "(1 ",
                DiagnosticKind.UnexpectedTokenError
            };

            yield return new object[] {
                "hello(1, 2",
                DiagnosticKind.UnexpectedTokenError
            };

            yield return new object[] {
                "data[36 + 1",
                DiagnosticKind.UnexpectedTokenError
            };

            yield return new object[] {
                "x.y.",
                DiagnosticKind.UnexpectedTokenError
            };
        }

        public static IEnumerable<object[]> GetStatementData()
        {
            yield return new object[] {
                "1",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "(1",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.UnexpectedTokenError,
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "1 + if * 5",
                DiagnosticKind.UnexpectedTokenError,
            };

            yield return new object[] {
                "var x = ;",
                DiagnosticKind.UnexpectedTokenError,
            };
        }

        public static IEnumerable<object[]> GetWholeProgramData()
        {
            yield return new object[] {
                @"
                var global = 5;
                const CONSTANT = 8;
                
                func main() {
                    global += CONSTANT;
                    println(CONSTANT);
                }
",
        };
        }
    }
}