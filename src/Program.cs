using System;
using System.Collections.Generic;

namespace Kyloe
{

    enum SyntaxTokenType
    {
        Invalid = 0,
        End = 1,
    }

    class SyntaxToken
    {
        public SyntaxToken(SyntaxTokenType type, object? value = null)
        {
            Type = type;
            Value = value;
        }

        public SyntaxTokenType Type { get; }
        public object? Value { get; }

        public override string ToString()
        {
            if (Value is null)
            {
                return Type.ToString();
            }
            else
            {
                return $"{Type}: {Value}";
            }
        }
    }

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
            return position < text.Length ? text[position] : '\0';
        }


        public SyntaxToken NextToken()
        {
            return new SyntaxToken(SyntaxTokenType.End);
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


    class Program
    {
        public static void Main()
        {
            while (true)
            {

                var input = Console.ReadLine();

                if (input is null)
                    return;

                if (input.StartsWith('$'))
                {
                    if (!EvaluteDollarCommand(input)) return;
                    continue;
                }

                Lexer lexer = new Lexer(input);

                foreach (var token in lexer.Tokens())
                {
                    Console.WriteLine(token);
                }
            }
        }


        private static bool EvaluteDollarCommand(string input)
        {
            if (input.StartsWith("$exit"))
            {
                return false;
            }
            else
            {
                Console.WriteLine($"Invalid dollar command: {input}");
                return true;
            }
        }
    }
}