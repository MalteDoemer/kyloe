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
        [InlineData("°", new DiagnosticKind[] { DiagnosticKind.UnknownCharacterError })]
        [InlineData("\"hello", new DiagnosticKind[] { DiagnosticKind.NeverClosedStringLiteralError })]
        [InlineData("/* hello", new DiagnosticKind[] { DiagnosticKind.NeverClosedBlockCommentError })]
        [InlineData("°/* ", new DiagnosticKind[] { DiagnosticKind.UnknownCharacterError, DiagnosticKind.NeverClosedBlockCommentError })]
        [InlineData("°'  ", new DiagnosticKind[] { DiagnosticKind.UnknownCharacterError, DiagnosticKind.NeverClosedStringLiteralError })]
        [InlineData("10000000000000000000000000000000000000000000", new DiagnosticKind[] { DiagnosticKind.InvalidIntLiteralError })]
        public void Test_Tokenization_With_Errors(string text, DiagnosticKind[] types)
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

        [Theory]
        [MemberData(nameof(GetCombinedTokenData))]
        public void Test_Combined_Tokens(string text, SyntaxTokenType t1, SyntaxTokenType t2)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(3, tokens.Length);

            Assert.Equal(t1, tokens[0].Type);
            Assert.Equal(t2, tokens[1].Type);
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


        public static IEnumerable<object[]> GetCombinedTokenData()
        {
            var typeArray = System.Enum.GetValues<SyntaxTokenType>();

            foreach (var t1 in typeArray)
            {
                foreach (var t2 in typeArray)
                {
    
                    switch (t1, t2)
                    {
                        case (SyntaxTokenType.Minus, SyntaxTokenType.Greater):
                        case (SyntaxTokenType.Plus, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Minus, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Star, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Percent, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Ampersand, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Pipe, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Hat, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Bang, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Less, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Greater, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Equals, SyntaxTokenType.Equals):
                        case (SyntaxTokenType.Ampersand, SyntaxTokenType.Ampersand):
                        case (SyntaxTokenType.Pipe, SyntaxTokenType.Pipe):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.Slash):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.Star):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.StarEquals):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.SlashEquals):
                        case (SyntaxTokenType.Plus, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Minus, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Star, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Slash, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Percent, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Ampersand, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Pipe, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Hat, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Bang, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Less, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Greater, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Minus, SyntaxTokenType.GreaterEqual):
                        case (SyntaxTokenType.Ampersand, SyntaxTokenType.AmpersandEquals):
                        case (SyntaxTokenType.Pipe, SyntaxTokenType.PipeEquals):
                        case (SyntaxTokenType.Equals, SyntaxTokenType.DoubleEqual):
                        case (SyntaxTokenType.Ampersand, SyntaxTokenType.DoubleAmpersand):
                        case (SyntaxTokenType.Pipe, SyntaxTokenType.DoublePipe):
                            continue;
                        default:
                            break;
                    }

                    var text1 = SyntaxInfo.GetTokenTypeString(t1);
                    var text2 = SyntaxInfo.GetTokenTypeString(t2);

                    if (text1 is null || text2 is null)
                        continue;

                    yield return new object[] { text1 + text2, t1, t2 };
                }
            }
        }

    }
}