using System.Collections.Generic;

namespace Kyloe
{
    class Lexer
    {

        private readonly string text;
        private int position;

        public Lexer(string text)
        {
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
                    return new SyntaxToken(SyntaxTokenType.Invalid);
                }

                AdvanceBy(1);
            }

            int end = position;

            AdvanceBy(1); // skip the terminating quote

            var str = text.Substring(start, end - start);

            return new SyntaxToken(SyntaxTokenType.StringLiteral, str);
        }

        private SyntaxToken SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(current))
                AdvanceBy(1);

            return NextToken();
        }

        private SyntaxToken LexNumber()
        {
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
                return new SyntaxToken(SyntaxTokenType.IntLiteral, integer);


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

            return new SyntaxToken(SyntaxTokenType.FloatLiteral, floatingPoint);
        }

        private SyntaxToken LexIdentOrKeyword()
        {
            int start = position;

            while (SyntaxInfo.IsIdentSubsequentChar(current))
            {
                AdvanceBy(1);
            }

            int end = position;

            string ident = text.Substring(start, end - start);

            if (SyntaxInfo.IsKeyword(ident) is SyntaxToken keywordToken)
                return keywordToken;
            else
                return new SyntaxToken(SyntaxTokenType.Identifier, ident);
        }

        private SyntaxToken? TryLexDoubleToken()
        {
            char c1 = current;
            char c2 = Peek(1);

            switch (c1, c2)
            {
                case ('-', '>'):
                    return new SyntaxToken(SyntaxTokenType.SmallArrow);
                case ('+', '='):
                    return new SyntaxToken(SyntaxTokenType.PlusEquals);
                case ('-', '='):
                    return new SyntaxToken(SyntaxTokenType.MinusEquals);
                case ('*', '='):
                    return new SyntaxToken(SyntaxTokenType.StarEquals);
                case ('/', '='):
                    return new SyntaxToken(SyntaxTokenType.SlashEquals);
                case ('%', '='):
                    return new SyntaxToken(SyntaxTokenType.PercentEquals);
                case ('&', '&'):
                    return new SyntaxToken(SyntaxTokenType.DoubleAmpersand);
                case ('|', '|'):
                    return new SyntaxToken(SyntaxTokenType.DoublePipe);
                case ('&', '='):
                    return new SyntaxToken(SyntaxTokenType.AmpersandEquals);
                case ('|', '='):
                    return new SyntaxToken(SyntaxTokenType.PipeEquals);
                case ('~', '='):
                    return new SyntaxToken(SyntaxTokenType.TildeEquals);
                case ('^', '='):
                    return new SyntaxToken(SyntaxTokenType.HatEquals);
                case ('=', '='):
                    return new SyntaxToken(SyntaxTokenType.DoubleEquals);
                case ('!', '='):
                    return new SyntaxToken(SyntaxTokenType.BangEquals);
                case ('<', '='):
                    return new SyntaxToken(SyntaxTokenType.LessEquals);
                case ('>', '='):
                    return new SyntaxToken(SyntaxTokenType.GreaterEquals);
                default:
                    return null;
            }
        }

        private SyntaxToken? TryLexSingleToken()
        {
            switch (current)
            {
                case '+':
                    return new SyntaxToken(SyntaxTokenType.Plus);
                case '-':
                    return new SyntaxToken(SyntaxTokenType.Minus);
                case '*':
                    return new SyntaxToken(SyntaxTokenType.Star);
                case '/':
                    return new SyntaxToken(SyntaxTokenType.Slash);
                case '%':
                    return new SyntaxToken(SyntaxTokenType.Percent);
                case '&':
                    return new SyntaxToken(SyntaxTokenType.Ampersand);
                case '|':
                    return new SyntaxToken(SyntaxTokenType.Pipe);
                case '~':
                    return new SyntaxToken(SyntaxTokenType.Tilde);
                case '^':
                    return new SyntaxToken(SyntaxTokenType.Hat);
                case '<':
                    return new SyntaxToken(SyntaxTokenType.Less);
                case '>':
                    return new SyntaxToken(SyntaxTokenType.Greater);
                case '=':
                    return new SyntaxToken(SyntaxTokenType.Equal);
                case '!':
                    return new SyntaxToken(SyntaxTokenType.Bang);
                case '(':
                    return new SyntaxToken(SyntaxTokenType.RightParen);
                case ')':
                    return new SyntaxToken(SyntaxTokenType.LeftParen);
                case '[':
                    return new SyntaxToken(SyntaxTokenType.RightSquare);
                case ']':
                    return new SyntaxToken(SyntaxTokenType.LeftSquare);
                case '{':
                    return new SyntaxToken(SyntaxTokenType.RightBracket);
                case '}':
                    return new SyntaxToken(SyntaxTokenType.LeftBracket);
                case ',':
                    return new SyntaxToken(SyntaxTokenType.Comma);
                case '.':
                    return new SyntaxToken(SyntaxTokenType.Dot);
                case ':':
                    return new SyntaxToken(SyntaxTokenType.Colon);
                case ';':
                    return new SyntaxToken(SyntaxTokenType.SemiColon);
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
                return new SyntaxToken(SyntaxTokenType.End);
            }
            else
            {
                return new SyntaxToken(SyntaxTokenType.Invalid, AdvanceBy(1));
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