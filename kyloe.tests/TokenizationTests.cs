using System.Collections.Generic;
using System.Linq;
using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;

namespace Kyloe.Tests.Tokenization
{
    public class TokenizationTests
    {
        [Theory]
        [InlineData("1 + 2")]
        [InlineData("arr[5]")]
        [InlineData("1 ( + 2 * 3 - 4 % 6 / . - \"hello\" 8 == x.y.z")]
        public void Test_Tokenization_Without_Errors(string text)
        {
            var (_, diagnostics) = SyntaxTree.Tokenize(text);
            DiagnosticAssert.NoErrors(diagnostics);
        }

        [Theory]
        [InlineData("°", new DiagnosticType[] { DiagnosticType.UnknownCharacterError })]
        [InlineData("\"hello", new DiagnosticType[] { DiagnosticType.NeverClosedStringLiteralError })]
        [InlineData("/* hello", new DiagnosticType[] { DiagnosticType.NeverClosedBlockCommentError })]
        [InlineData("°/* ", new DiagnosticType[] { DiagnosticType.UnknownCharacterError, DiagnosticType.NeverClosedBlockCommentError })]
        [InlineData("°'  ", new DiagnosticType[] { DiagnosticType.UnknownCharacterError, DiagnosticType.NeverClosedStringLiteralError })]
        [InlineData("10000000000000000000000000000000000000000000", new DiagnosticType[] { DiagnosticType.InvalidIntLiteralError })]
        public void Test_Tokenization_With_Errors(string text, DiagnosticType[] types)
        {
            var (_, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.HasAll(diagnostics, types);
        }


        [Theory]
        [MemberData(nameof(GetSimpleTokenData))]
        public void Test_Simple_Tokens(string text, SyntaxTokenType type)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(2, tokens.Length);
            Assert.Equal(type, tokens.First().Type);
            Assert.Equal(SyntaxTokenType.End, tokens.Last().Type);
        }

        [Theory]
        [MemberData(nameof(GetComplexTokenData))]
        public void Test_Complex_Tokens(string text, SyntaxTokenType type, object value)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(2, tokens.Length);
            Assert.Equal(type, tokens.First().Type);
            Assert.Equal(value, tokens.First().Value);
            Assert.Equal(SyntaxTokenType.End, tokens.Last().Type);
        }


        public static IEnumerable<object[]> GetSimpleTokenData()
        {
            var typeArray = System.Enum.GetValues<SyntaxTokenType>();

            foreach (var type in typeArray)
            {
                var text = SyntaxInfo.GetTokenTypeString(type);

                if (text is not null)
                    yield return new object[] { text, type };
            }
        }


        public static IEnumerable<object[]> GetComplexTokenData()
        {
            var intLiteralData = new List<(string, long)> {
                ("1", 1),
                ("100", 100),
                ("1234567890", 1234567890),
                ("0", 0),
                ("00001", 1),
                ("9223372036854775807", 9223372036854775807L),
            };

            var floatLiteralData = new List<(string, double)> {
                ("1.0", 1.0),
                ("01.00", 1.0),
                ("1234567890.1234567890", 1234567890.1234567890),
                ("0.0", 0.0),
                ("00001.100000", 1.1),
                ("0.000000000000000000000000000009", 0.000000000000000000000000000009),
                ("9000000000000000000000000000000.0", 9000000000000000000000000000000.0),
            };

            var boolLiteralData = new List<(string, bool)> {
                ("true", true),
                ("false", false),
            };

            var stringLiteralData = new List<(string, string)> {
                ("\"\"", ""),
                ("''", ""),

                ("\"hello\"", "hello"),
                ("'hello'", "hello"),

                ("\"'\"", "'"),
                ("'\"'", "\""),


                ("\"1234 + 0.05\"", "1234 + 0.05"),
                ("'1234 + 0.05'", "1234 + 0.05"),


                ("\"// not a comment\"", "// not a comment"),
                ("'// not a comment'", "// not a comment"),

                ("\"hey /* not a block comment */ there\"", "hey /* not a block comment */ there"),
                ("'hey /* not a block comment */ there'", "hey /* not a block comment */ there"),


                ("\"°\"", "°"),
                ("'°'", "°"),

                ("\"œ\"", "œ"),
                ("'œ'", "œ"),

                ("\"├\"", "├"),
                ("'├'", "├"),

                ("\"ض\"", "ض"),
                ("'ض'", "ض"),
            };


            foreach (var (t, l) in intLiteralData)
                yield return new object[] { t, SyntaxTokenType.IntLiteral, l };

            foreach (var (t, d) in floatLiteralData)
                yield return new object[] { t, SyntaxTokenType.FloatLiteral, d };

            foreach (var (t, b) in boolLiteralData)
                yield return new object[] { t, SyntaxTokenType.BoolLiteral, b };

            foreach (var (t, s) in stringLiteralData)
                yield return new object[] { t, SyntaxTokenType.StringLiteral, s };
        }

    }
}