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
        public void Test_Lexing_Without_Errors(string text)
        {
            var terminals = SyntaxTree.Terminals(text);
            Assert.DoesNotContain(SyntaxTokenKind.Error, terminals.Select(t => t.Kind));
        }

        [Theory]
        [MemberData(nameof(GetSimpleTerminalData))]
        public void Test_Simple_Terminals(string text, SyntaxTokenKind kind)
        {
            var terminals = SyntaxTree.Terminals(text);

            Assert.DoesNotContain(SyntaxTokenKind.Error, terminals.Select(t => t.Kind));
            Assert.Equal(2, terminals.Length);
            Assert.Equal(kind, terminals.First().Kind);
            Assert.Equal(SyntaxTokenKind.End, terminals.Last().Kind);
        }

        [Theory]
        [MemberData(nameof(GetComplexTerminalData))]
        public void Test_Complex_Terminals(string text, SyntaxTokenKind kind)
        {
            var terminals = SyntaxTree.Terminals(text);

            Assert.DoesNotContain(SyntaxTokenKind.Error, terminals.Select(t => t.Kind));

            Assert.Equal(2, terminals.Length);
            Assert.Equal(kind, terminals.First().Kind);
            Assert.Equal(SyntaxTokenKind.End, terminals.Last().Kind);
        }

        [Theory]
        [MemberData(nameof(GetCombinedTokenData))]
        public void Test_Combined_Tokens(string text, SyntaxTokenKind t1, SyntaxTokenKind t2)
        {
            var terminals = SyntaxTree.Terminals(text);

            Assert.DoesNotContain(SyntaxTokenKind.Error, terminals.Select(t => t.Kind));

            Assert.Equal(3, terminals.Length);
            Assert.Equal(t1, terminals[0].Kind);
            Assert.Equal(t2, terminals[1].Kind);
            Assert.Equal(SyntaxTokenKind.End, terminals.Last().Kind);
        }


        public static IEnumerable<object[]> GetSimpleTerminalData()
        {
            var kindArray = System.Enum.GetValues<SyntaxTokenKind>();

            foreach (var kind in kindArray)
            {
                var text = SyntaxInfo.GetSimpleTerminalString(kind);

                if (text is not null)
                    yield return new object[] { text, kind };
            }
        }

        public static IEnumerable<object[]> GetComplexTerminalData()
        {
            var intLiteralData = new List<string> {
                "1",
                "100",
                "1234567890",
                "0",
                "00001",
                "9223372036854775807",
            };

            var floatLiteralData = new List<string> {
                "1.0",
                "01.00",
                "1234567890.1234567890",
                "0.0",
                "00001.100000",
                "0.000000000000000000000000000009",
                "9000000000000000000000000000000.0",
            };

            var boolLiteralData = new List<string> {
                "true",
                "false",
            };

            var stringLiteralData = new List<string> {
                "\"\"",
                "''",
                "\"hello\"",
                "'hello'",
                "\"'\"",
                "'\"'",
                "\"1234 + 0.05\"",
                "'1234 + 0.05'",
                "\"// not a comment\"",
                "'// not a comment'",
                "\"hey /* not a block comment */ there\"",
                "'hey /* not a block comment */ there'",
                "\"°\"",
                "'°'",
                "\"œ\"",
                "'œ'",
                "\"├\"",
                "'├'",
                "\"ض\"",
                "'ض'",
            };

            foreach (var t in intLiteralData)
                yield return new object[] { t, SyntaxTokenKind.Int };

            foreach (var t in floatLiteralData)
                yield return new object[] { t, SyntaxTokenKind.Float };

            foreach (var t in boolLiteralData)
                yield return new object[] { t, SyntaxTokenKind.Bool };

            foreach (var t in stringLiteralData)
                yield return new object[] { t, SyntaxTokenKind.String };
        }

        public static IEnumerable<object[]> GetCombinedTokenData()
        {
            var kindArray = System.Enum.GetValues<SyntaxTokenKind>();

            foreach (var kind1 in kindArray)
            {
                foreach (var kind2 in kindArray)
                {
                    switch (kind1, kind2)
                    {
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.Greater):
                        case (SyntaxTokenKind.Plus, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Minus, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Star, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Percent, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Hat, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Bang, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Less, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Greater, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Equal, SyntaxTokenKind.Equal):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.Ampersand):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.Pipe):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Slash):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.Star):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.StarEqual):
                        case (SyntaxTokenKind.Slash, SyntaxTokenKind.SlashEqual):
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
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.AmpersandEqual):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.PipeEqual):
                        case (SyntaxTokenKind.Equal, SyntaxTokenKind.DoubleEqual):
                        case (SyntaxTokenKind.Ampersand, SyntaxTokenKind.DoubleAmpersand):
                        case (SyntaxTokenKind.Pipe, SyntaxTokenKind.DoublePipe):
                            continue;
                        default:
                            break;
                    }

                    var text1 = SyntaxInfo.GetSimpleTerminalString(kind1);
                    var text2 = SyntaxInfo.GetSimpleTerminalString(kind2);

                    if (text1 is null || text2 is null)
                        continue;

                    
                    // we have to add spaces if it is a keyword
                    // otherwise it would not be recognized as such
                    if (kind1.IsKeyword() || kind2.IsKeyword())
                    {
                        var text = $"{text1} /* hi */ {text2}";
                        yield return new object[] { text, kind1, kind2 };
                    }
                    else
                    {
                        yield return new object[] { text1 + text2, kind1, kind2 };
                    }
                }
            }
        }
    }
}