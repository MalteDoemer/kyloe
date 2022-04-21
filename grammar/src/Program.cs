using System;

namespace Kyloe.Grammar
{
    public class Program
    {
        private static string text = @"


Primary = ° (LParen, Expr, RParen);
";

        public static void Main()
        {
            var lexer = new GrammarLexer(text);

            foreach (var token in lexer.Tokens())
            {
                Console.WriteLine(token);
            }
        }
    }
}