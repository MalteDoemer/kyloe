using System.IO;
using System.Collections.Generic;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using System.Text;

namespace Kyloe.Syntax
{
    internal class Lexer
    {
        private readonly DiagnosticCollector diagnostics;
        private readonly TextReader reader;
        private int position;

        public Lexer(TextReader reader, DiagnosticCollector diagnostics)
        {
            this.diagnostics = diagnostics;
            this.reader = reader;
            this.position = 0;


            int r1 = reader.Read();
            current = r1 == -1 ? '\0' : (char)r1;

            int r2 = reader.Read();
            next = r2 == -1 ? '\0' : (char)r2;

        }

        private char current;

        private char next;

        private char Advance()
        {
            if (current == '\0')
                return '\0';

            char ret = current;
            current = next;
            position += 1;

            if (next == '\0')
                return ret;

            int res = reader.Read();
            next = res == -1 ? '\0' : (char)res;

            return ret;
        }

        private void Skip(int num)
        {
            for (int i = 0; i < num; i++)
                Advance();
        }

        private SyntaxToken LexStringLiteral()
        {
            int start = position;
            var quote = Advance();

            var builder = new StringBuilder();

            while (current != quote)
            {
                // TODO: handle escape sequences

                if (current == '\0')
                {
                    var token = new SyntaxToken(SyntaxTokenKind.Invalid, SourceLocation.FromBounds(start, position));
                    diagnostics.Add(new NeverClosedStringLiteralError(token));
                    return token;
                }

                builder.Append(Advance());
            }

            Skip(1); // skip the terminating quote

            var location = SourceLocation.FromBounds(start, position);

            return new SyntaxToken(SyntaxTokenKind.StringLiteral, location, builder.ToString());
        }

        private SyntaxToken SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(current))
                Skip(1);

            return NextToken();
        }

        private SyntaxToken SkipLineComment()
        {
            while (!(current == '\n' || current == '\0'))
                Skip(1);


            return NextToken();
        }

        private SyntaxToken SkipBlockComment()
        {
            var start = position;

            while (!(current == '*' && next == '/'))
            {
                if (current == '\0')
                {
                    var token = new SyntaxToken(SyntaxTokenKind.Invalid, SourceLocation.FromBounds(start, position));
                    diagnostics.Add(new NeverClosedBlockCommentError(token));
                    return token;
                }
                Skip(1);
            }

            Skip(2); // skip the closing chars: */

            return NextToken();
        }

        private SyntaxToken LexNumber()
        {
            int start = position;
            var builder = new StringBuilder();

            while (char.IsNumber(current))
                builder.Append(Advance());

            if (!(current == '.' && char.IsNumber(next)))
            {
                var location = SourceLocation.FromBounds(start, position);
                var str = builder.ToString();

                if (long.TryParse(str, out long value))
                {
                    return new SyntaxToken(SyntaxTokenKind.IntLiteral, location, value);
                }
                else
                {
                    var token = new SyntaxToken(SyntaxTokenKind.Invalid, location, str);
                    diagnostics.Add(new InvalidIntLiteralError(token));
                    return token;
                }

            }
            else
            {
                builder.Append(Advance()); // add the decimal point

                while (char.IsNumber(current))
                    builder.Append(Advance());

                var location = SourceLocation.FromBounds(start, position);
                var str = builder.ToString();

                if (double.TryParse(str, out double value))
                {
                    return new SyntaxToken(SyntaxTokenKind.FloatLiteral, location, value);
                }
                else
                {
                    var token = new SyntaxToken(SyntaxTokenKind.FloatLiteral, location, str);
                    diagnostics.Add(new InvalidFloatLiteralError(token));
                    return token;
                }
            }
        }

        private SyntaxToken LexIdentOrKeyword()
        {
            int start = position;

            var builder = new StringBuilder();

            while (SyntaxInfo.IsIdentSubsequentChar(current))
                builder.Append(Advance());

            string ident = builder.ToString();
            var location = SourceLocation.FromBounds(start, position);

            switch (ident)
            {
                case "true":
                    return new SyntaxToken(SyntaxTokenKind.BoolLiteral, location, true);
                case "false":
                    return new SyntaxToken(SyntaxTokenKind.BoolLiteral, location, false);
                case "var":
                    return new SyntaxToken(SyntaxTokenKind.VarKeyword, location);
                case "if":
                    return new SyntaxToken(SyntaxTokenKind.IfKeyword, location);
                case "else":
                    return new SyntaxToken(SyntaxTokenKind.ElseKeyword, location);
                case "const":
                    return new SyntaxToken(SyntaxTokenKind.ConstKeyword, location);
                case "func":
                    return new SyntaxToken(SyntaxTokenKind.FuncKeyword, location);
                default:
                    return new SyntaxToken(SyntaxTokenKind.Identifier, location, ident);
            }
        }

        private SyntaxToken? TryLexDoubleToken()
        {
            char c1 = current;
            char c2 = next;

            var location = SourceLocation.FromLength(position, 2);

            switch (c1, c2)
            {
                case ('-', '>'):
                    return new SyntaxToken(SyntaxTokenKind.SmallArrow, location);
                case ('+', '='):
                    return new SyntaxToken(SyntaxTokenKind.PlusEquals, location);
                case ('-', '='):
                    return new SyntaxToken(SyntaxTokenKind.MinusEquals, location);
                case ('*', '='):
                    return new SyntaxToken(SyntaxTokenKind.StarEquals, location);
                case ('/', '='):
                    return new SyntaxToken(SyntaxTokenKind.SlashEquals, location);
                case ('%', '='):
                    return new SyntaxToken(SyntaxTokenKind.PercentEquals, location);
                case ('&', '&'):
                    return new SyntaxToken(SyntaxTokenKind.DoubleAmpersand, location);
                case ('|', '|'):
                    return new SyntaxToken(SyntaxTokenKind.DoublePipe, location);
                case ('&', '='):
                    return new SyntaxToken(SyntaxTokenKind.AmpersandEquals, location);
                case ('|', '='):
                    return new SyntaxToken(SyntaxTokenKind.PipeEquals, location);
                case ('^', '='):
                    return new SyntaxToken(SyntaxTokenKind.HatEquals, location);
                case ('=', '='):
                    return new SyntaxToken(SyntaxTokenKind.DoubleEqual, location);
                case ('!', '='):
                    return new SyntaxToken(SyntaxTokenKind.NotEqual, location);
                case ('<', '='):
                    return new SyntaxToken(SyntaxTokenKind.LessEqual, location);
                case ('>', '='):
                    return new SyntaxToken(SyntaxTokenKind.GreaterEqual, location);
                default:
                    return null;
            }
        }

        private SyntaxToken? TryLexSingleToken()
        {
            var location = SourceLocation.FromLength(position, 1);

            switch (current)
            {
                case '+':
                    return new SyntaxToken(SyntaxTokenKind.Plus, location);
                case '-':
                    return new SyntaxToken(SyntaxTokenKind.Minus, location);
                case '*':
                    return new SyntaxToken(SyntaxTokenKind.Star, location);
                case '/':
                    // A slash could also mean a line comment, or a block comment.
                    // In that case just return null, it will be handled later.
                    if (next == '/' || next == '*')
                        return null;
                    return new SyntaxToken(SyntaxTokenKind.Slash, location);
                case '%':
                    return new SyntaxToken(SyntaxTokenKind.Percent, location);
                case '&':
                    return new SyntaxToken(SyntaxTokenKind.Ampersand, location);
                case '|':
                    return new SyntaxToken(SyntaxTokenKind.Pipe, location);
                case '~':
                    return new SyntaxToken(SyntaxTokenKind.Tilde, location);
                case '^':
                    return new SyntaxToken(SyntaxTokenKind.Hat, location);
                case '<':
                    return new SyntaxToken(SyntaxTokenKind.Less, location);
                case '>':
                    return new SyntaxToken(SyntaxTokenKind.Greater, location);
                case '=':
                    return new SyntaxToken(SyntaxTokenKind.Equals, location);
                case '!':
                    return new SyntaxToken(SyntaxTokenKind.Bang, location);
                case '(':
                    return new SyntaxToken(SyntaxTokenKind.LeftParen, location);
                case ')':
                    return new SyntaxToken(SyntaxTokenKind.RightParen, location);
                case '[':
                    return new SyntaxToken(SyntaxTokenKind.LeftSquare, location);
                case ']':
                    return new SyntaxToken(SyntaxTokenKind.RightSquare, location);
                case '{':
                    return new SyntaxToken(SyntaxTokenKind.LeftCurly, location);
                case '}':
                    return new SyntaxToken(SyntaxTokenKind.RightCurly, location);
                case ',':
                    return new SyntaxToken(SyntaxTokenKind.Comma, location);
                case '.':
                    return new SyntaxToken(SyntaxTokenKind.Dot, location);
                case ':':
                    return new SyntaxToken(SyntaxTokenKind.Colon, location);
                case ';':
                    return new SyntaxToken(SyntaxTokenKind.SemiColon, location);
                default:
                    return null;
            }
        }

        public SyntaxToken NextToken()
        {
            if (TryLexDoubleToken() is SyntaxToken doubleToken)
            {
                Skip(2);
                return doubleToken;
            }
            else if (TryLexSingleToken() is SyntaxToken singleToken)
            {
                Skip(1);
                return singleToken;
            }
            else if (current == '/' && next == '/')
            {
                return SkipLineComment();
            }
            else if (current == '/' && next == '*')
            {
                return SkipBlockComment();
            }
            else if (char.IsWhiteSpace(current))
            {
                return SkipWhiteSpace();
            }
            else if (char.IsNumber(current))
            {
                return LexNumber();
            }
            else if (SyntaxInfo.IsIdentStartChar(current))
            {
                return LexIdentOrKeyword();
            }
            else if (SyntaxInfo.IsStringQuote(current))
            {
                return LexStringLiteral();
            }
            else if (current == '\0')
            {
                return new SyntaxToken(SyntaxTokenKind.End, SourceLocation.FromLength(position, 1));
            }
            else
            {
                var token = new SyntaxToken(SyntaxTokenKind.Invalid, SourceLocation.FromLength(position, 1), Advance());
                diagnostics.Add(new UnknownCharacterError(token));
                return token;
            }
        }

        public IEnumerable<SyntaxToken> Tokens()
        {
            SyntaxToken token;

            do
            {
                token = NextToken();
                yield return token;
            } while (token.Kind != SyntaxTokenKind.End);
        }
    }
}