using System;

namespace Kyloe.Grammar
{
    public class Program
    {
        private static string text = @"


Primary = (LParen, Expr, RParen) | #;
";

        public static void Main()
        {
            var parser = new GrammarParser(text);

            var grammar = parser.Parse();

            foreach (var stmt in grammar.Statements) 
            {
                Console.WriteLine(stmt);
            }
        }
    }
}