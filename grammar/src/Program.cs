using System;
using System.IO;

namespace Kyloe.Grammar
{
    public class Program
    {

        public static void Main(string[]? args)
        {
            if (args is null || args.Length < 1)
            {
                Console.WriteLine("error: expected a grammar file as first argument");
                return;
            }

            var text = File.ReadAllText(args[0]);

            var parser = new GrammarParser(text);
            var binder = new GrammarBinder(parser.Parse());

            var grammar = binder.Bind();


            foreach (var rule in grammar.Rules.Values)
            {
                Console.WriteLine(rule);
            }
        }
    }
}