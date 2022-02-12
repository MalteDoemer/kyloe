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
                    var token = new SyntaxToken(SyntaxTokenType.Invalid, SourceLocation.FromBounds(start, position));
                    diagnostics.Add(new NeverClosedStringLiteralError(token));
                    return token;
                }

                builder.Append(Advance());
            }

            Skip(1); // skip the terminating quote

            var location = SourceLocation.FromBounds(start, position);

            return new SyntaxToken(SyntaxTokenType.StringLiteral, location, builder.ToString());
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
                    var token = new SyntaxToken(SyntaxTokenType.Invalid, SourceLocation.FromBounds(start, position));
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
            long integer = 0;

            // First parse the number as an int
            // but if there is a dot in between
            // switch to float.

            // TODO: handle non-ascii numbers

            while (char.IsNumber(current))
            {
                int digit = current - '0';
                integer = integer * 10 + digit;
                Skip(1);
            }

            if (!(current == '.' && char.IsNumber(next)))
                return new SyntaxToken(SyntaxTokenType.IntLiteral, SourceLocation.FromBounds(start, position), integer);


            Skip(1); // skip the decimal point

            double floatingPoint = integer;
            long factor = 10;

            while (char.IsNumber(current))
            {
                int digit = current - '0';
                floatingPoint += digit / (double)factor;
                factor *= 10;
                Skip(1);
            }

            return new SyntaxToken(SyntaxTokenType.FloatLiteral, SourceLocation.FromBounds(start, position), floatingPoint);
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
                    return new SyntaxToken(SyntaxTokenType.BoolLiteral, location, true);
                case "false":
                    return new SyntaxToken(SyntaxTokenType.BoolLiteral, location, false);
                default:
                    return new SyntaxToken(SyntaxTokenType.Identifier, location, ident);
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
                    return new SyntaxToken(SyntaxTokenType.SmallArrow, location);
                case ('+', '='):
                    return new SyntaxToken(SyntaxTokenType.PlusEquals, location);
                case ('-', '='):
                    return new SyntaxToken(SyntaxTokenType.MinusEquals, location);
                case ('*', '='):
                    return new SyntaxToken(SyntaxTokenType.StarEquals, location);
                case ('/', '='):
                    return new SyntaxToken(SyntaxTokenType.SlashEquals, location);
                case ('%', '='):
                    return new SyntaxToken(SyntaxTokenType.PercentEquals, location);
                case ('&', '&'):
                    return new SyntaxToken(SyntaxTokenType.DoubleAmpersand, location);
                case ('|', '|'):
                    return new SyntaxToken(SyntaxTokenType.DoublePipe, location);
                case ('&', '='):
                    return new SyntaxToken(SyntaxTokenType.AmpersandEquals, location);
                case ('|', '='):
                    return new SyntaxToken(SyntaxTokenType.PipeEquals, location);
                case ('^', '='):
                    return new SyntaxToken(SyntaxTokenType.HatEquals, location);
                case ('=', '='):
                    return new SyntaxToken(SyntaxTokenType.DoubleEqual, location);
                case ('!', '='):
                    return new SyntaxToken(SyntaxTokenType.NotEqual, location);
                case ('<', '='):
                    return new SyntaxToken(SyntaxTokenType.LessEqual, location);
                case ('>', '='):
                    return new SyntaxToken(SyntaxTokenType.GreaterEqual, location);
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
                    return new SyntaxToken(SyntaxTokenType.Plus, location);
                case '-':
                    return new SyntaxToken(SyntaxTokenType.Minus, location);
                case '*':
                    return new SyntaxToken(SyntaxTokenType.Star, location);
                case '/':
                    // A slash could also mean a line comment, or a block comment.
                    // In that case just return null, it will be handled later.
                    if (next == '/' || next == '*')
                        return null;
                    return new SyntaxToken(SyntaxTokenType.Slash, location);
                case '%':
                    return new SyntaxToken(SyntaxTokenType.Percent, location);
                case '&':
                    return new SyntaxToken(SyntaxTokenType.Ampersand, location);
                case '|':
                    return new SyntaxToken(SyntaxTokenType.Pipe, location);
                case '~':
                    return new SyntaxToken(SyntaxTokenType.Tilde, location);
                case '^':
                    return new SyntaxToken(SyntaxTokenType.Hat, location);
                case '<':
                    return new SyntaxToken(SyntaxTokenType.Less, location);
                case '>':
                    return new SyntaxToken(SyntaxTokenType.Greater, location);
                case '=':
                    return new SyntaxToken(SyntaxTokenType.Equals, location);
                case '!':
                    return new SyntaxToken(SyntaxTokenType.Bang, location);
                case '(':
                    return new SyntaxToken(SyntaxTokenType.LeftParen, location);
                case ')':
                    return new SyntaxToken(SyntaxTokenType.RightParen, location);
                case '[':
                    return new SyntaxToken(SyntaxTokenType.LeftSquare, location);
                case ']':
                    return new SyntaxToken(SyntaxTokenType.RightSquare, location);
                case '{':
                    return new SyntaxToken(SyntaxTokenType.LeftBracket, location);
                case '}':
                    return new SyntaxToken(SyntaxTokenType.RightBracket, location);
                case ',':
                    return new SyntaxToken(SyntaxTokenType.Comma, location);
                case '.':
                    return new SyntaxToken(SyntaxTokenType.Dot, location);
                case ':':
                    return new SyntaxToken(SyntaxTokenType.Colon, location);
                case ';':
                    return new SyntaxToken(SyntaxTokenType.SemiColon, location);
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
                return new SyntaxToken(SyntaxTokenType.End, SourceLocation.FromLength(position, 1));
            }
            else
            {
                var token = new SyntaxToken(SyntaxTokenType.Invalid, SourceLocation.FromLength(position, 1), Advance());
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
            } while (token.Type != SyntaxTokenType.End);
        }
    }
}