using System.Collections.Generic;
using System.Linq;

namespace Kyloe.Grammar
{
    internal sealed class GrammarLexer
    {
        private readonly string text;

        private int pos;
        private int line;
        private int column;

        public GrammarLexer(string text)
        {
            this.text = text;
            this.pos = 0;
            this.line = 1;
            this.column = 1;
        }

        private char current => text.ElementAtOrDefault(pos);
        private char next => text.ElementAtOrDefault(pos + 1);

        private GrammarLocation LocationFromLength(int length) => GrammarLocation.FromLength(pos, length, line, column);

        private char Advance()
        {
            var temp = current;

            if (temp == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }

            pos++;
            return temp;
        }

        private void Skip(int off)
        {
            for (int i = 0; i < off; i++)
                Advance();
        }

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(current))
                Skip(1);
        }

        private void SkipComment()
        {
            var start = LocationFromLength(0);

            Skip(2);

            while (!(current == '*' && next == ')'))
            {
                if (current == default(char))
                    throw new GrammarException($"never closed comment", GrammarLocation.FromBounds(start.Start, pos, start.Line, start.Column));
                Skip(1);
            }

            Skip(2);
        }


        private GrammarToken LexQuoted()
        {
            var quote = Advance();

            var start = LocationFromLength(0);

            while (current != quote && current != default(char))
                Skip(1);

            var location = GrammarLocation.FromBounds(start.Start, pos, start.Line, start.Column);

            if (current == default(char))
                throw new GrammarException($"never closed quote: '{quote}'", location);
            else
                Skip(1);

            var str = text.Substring(location.Start, location.Length);

            return new GrammarToken(GrammarTokenKind.String, str, location);
        }

        private GrammarToken LexIdentifier()
        {
            var start = LocationFromLength(0);

            while (char.IsLetterOrDigit(current))
                Skip(1);

            var location = GrammarLocation.FromBounds(start.Start, pos, start.Line, start.Column);

            var str = text.Substring(location.Start, location.Length);

            if (IsReservedName(str))
                throw new GrammarException($"the name '{str}' is reserved and cannot be used", location);

            return new GrammarToken(GrammarTokenKind.Identifier, str, location);
        }

        private bool IsReservedName(string str)
        {
            return string.Equals(str, "Error")
                || string.Equals(str, "Epsilon")
                || string.Equals(str, "End");
        }

        private GrammarToken NextToken()
        {
            SkipWhiteSpace();

            if (current == default(char))
            {
                return new GrammarToken(GrammarTokenKind.End, "", LocationFromLength(0));
            }
            else if (current == '(' && next == '*')
            {
                SkipComment();
                return NextToken();
            }
            else if (current == '(')
            {
                var token = new GrammarToken(GrammarTokenKind.LeftParen, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == ')')
            {
                var token = new GrammarToken(GrammarTokenKind.RightParen, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == '=')
            {
                var token = new GrammarToken(GrammarTokenKind.Equal, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == ';')
            {
                var token = new GrammarToken(GrammarTokenKind.SemiColon, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == ',')
            {
                var token = new GrammarToken(GrammarTokenKind.Comma, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == '|')
            {
                var token = new GrammarToken(GrammarTokenKind.Or, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == '#')
            {
                var token = new GrammarToken(GrammarTokenKind.Hash, current.ToString(), LocationFromLength(1));
                Advance();
                return token;
            }
            else if (current == '\'')
            {
                return LexQuoted();
            }
            else if (char.IsLetter(current))
            {
                return LexIdentifier();
            }
            else
            {
                var message = $"invalid character: '0x{((int)current).ToString("X")}'";
                var location = LocationFromLength(1);
                throw new GrammarException(message, location);
            }
        }

        public IEnumerable<GrammarToken> Tokens()
        {
            GrammarToken token;

            do
            {
                token = NextToken();
                yield return token;
            } while (token.Kind != GrammarTokenKind.End);
        }
    }
}