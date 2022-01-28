using System.Collections.Generic;
using Kyloe.Diagnostics;
using Kyloe.Text;

namespace Kyloe.Syntax
{
    class Lexer
    {
        private readonly DiagnosticCollecter diagnostics;
        private readonly string text;
        private int position;

        public Lexer(string text, DiagnosticCollecter diagnostics)
        {
            this.diagnostics = diagnostics;
            this.text = text;
            this.position = 0;
        }

        private char current => Peek(0);

        private char Peek(int offset)
        {
            var pos = position + offset;
            return pos < text.Length ? text[pos] : '\0';
        }

        /// Advances the position by offset and returns the current char before the advance.
        private char AdvanceBy(int offset)
        {
            char c = current;
            position += offset;
            return c;
        }

        private SyntaxToken LexStringLiteral()
        {
            var quote = AdvanceBy(1);
            int start = position;

            while (current != quote)
            {
                // TODO: handle escape sequences

                if (current == '\0')
                {
                    diagnostics.Add(new NeverClosedStringLiteralError());
                    return new SyntaxToken(SyntaxTokenType.Invalid, SourceLocation.FromBounds(start, position));
                }

                AdvanceBy(1);
            }

            int length = position - start;

            AdvanceBy(1); // skip the terminating quote

            var str = text.Substring(start, length);
            var location = SourceLocation.FromLength(start, length);

            return new SyntaxToken(SyntaxTokenType.StringLiteral, location, str);
        }

        private SyntaxToken SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(current))
                AdvanceBy(1);

            return NextToken();
        }

        private SyntaxToken LexNumber()
        {
            int start = position;
            long integer = 0;

            // first parse the number as an int
            // but if there is a dot in between
            // switch to float

            // TODO: handle non-ascii numbers

            while (char.IsNumber(current))
            {
                int digit = current - '0';
                integer = integer * 10 + digit;
                AdvanceBy(1);
            }

            if (!(current == '.' && char.IsNumber(Peek(1))))
                return new SyntaxToken(SyntaxTokenType.IntLiteral, SourceLocation.FromBounds(start, position), integer);


            AdvanceBy(1); // skip the decimal point

            double floatingPoint = integer;
            long factor = 10;

            while (char.IsNumber(current))
            {
                int digit = current - '0';
                floatingPoint += digit / (double)factor;
                factor *= 10;
                AdvanceBy(1);
            }

            return new SyntaxToken(SyntaxTokenType.FloatLiteral, SourceLocation.FromBounds(start, position), floatingPoint);
        }

        private SyntaxToken LexIdentOrKeyword()
        {
            int start = position;

            while (SyntaxInfo.IsIdentSubsequentChar(current))
            {
                AdvanceBy(1);
            }

            int length = position - start;

            string ident = text.Substring(start, length);
            var location = SourceLocation.FromLength(start, length);

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
            char c2 = Peek(1);

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
                AdvanceBy(2);
                return doubleToken;
            }
            else if (TryLexSingleToken() is SyntaxToken singleToken)
            {
                AdvanceBy(1);
                return singleToken;
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
                return new SyntaxToken(SyntaxTokenType.End, SourceLocation.FromLength(position, 0));
            }
            else
            {
                diagnostics.Add(new UnknownCharacterError(current));
                return new SyntaxToken(SyntaxTokenType.Invalid, SourceLocation.FromLength(position, 1), AdvanceBy(1));
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