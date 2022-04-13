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
        public void Test_Simple_Tokens(string text, SyntaxTokenKind kind)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(2, tokens.Length);
            Assert.Equal(kind, tokens.First().Kind);
            Assert.Equal(SyntaxTokenKind.End, tokens.Last().Kind);
        }

        [Theory]
        [MemberData(nameof(GetComplexTokenData))]
        public void Test_Complex_Tokens(string text, SyntaxTokenKind kind, object value)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(2, tokens.Length);
            Assert.Equal(kind, tokens.First().Kind);
            Assert.Equal(value, tokens.First().Value);
            Assert.Equal(SyntaxTokenKind.End, tokens.Last().Kind);
        }

        [Theory]
        [MemberData(nameof(GetCombinedTokenData))]
        public void Test_Combined_Tokens(string text, SyntaxTokenKind t1, SyntaxTokenKind t2)
        {
            var (tokens, diagnostics) = SyntaxTree.Tokenize(text);

            DiagnosticAssert.NoErrors(diagnostics);

            Assert.Equal(3, tokens.Length);

            Assert.Equal(t1, tokens[0].Kind);
            Assert.Equal(t2, tokens[1].Kind);
            Assert.Equal(SyntaxTokenKind.End, tokens.Last().Kind);
        }


        public static IEnumerable<object[]> GetSimpleTokenData()
        {
            var typeArray = System.Enum.GetValues<SyntaxTokenKind>();

            foreach (var type in typeArray)
            {
                var text = SyntaxInfo.GetTokenKindString(type);

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
                yield return new object[] { t, SyntaxTokenKind.IntLiteral, l };

            foreach (var (t, d) in floatLiteralData)
                yield return new object[] { t, SyntaxTokenKind.FloatLiteral, d };

            foreach (var (t, b) in boolLiteralData)
                yield return new object[] { t, SyntaxTokenKind.BoolLiteral, b };

            foreach (var (t, s) in stringLiteralData)
                yield return new object[] { t, SyntaxTokenKind.StringLiteral, s };
        }


        public static IEnumerable<object[]> GetCombinedTokenData()
        {
            var typeArray = System.Enum.GetValues<SyntaxTokenKind>();

            foreach (var t1 in typeArray)
            {
                foreach (var t2 in typeArray)
                {
    
                    switch (t1, t2)
                    {
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.Greater):
                        case (SyntaxTokenKind.Plus, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Star, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Percent, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Hat, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Bang, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Less, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Greater, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Equals, SyntaxTokenKind.Equals):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.Ampersand):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.Pipe):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Slash):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Star):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.StarEquals):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.SlashEquals):
                        case (SyntaxTokenKind.Plus, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Star, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Percent, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Hat, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Bang, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Less, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Greater, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.GreaterEqual):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.AmpersandEquals):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.PipeEquals):
                        case (SyntaxTokenKind.Equals, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.DoubleAmpersand):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.DoublePipe):
                            continue;
                        default:
                            break;
                    }

                    var text1 = SyntaxInfo.GetTokenKindString(t1);
                    var text2 = SyntaxInfo.GetTokenKindString(t2);

                    if (text1 is null || text2 is null)
                        continue;

                    yield return new object[] { text1 + text2, t1, t2 };
                }
            }
        }

    }
}