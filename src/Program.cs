using System;
using System.Text;
using Kyloe.Syntax;

namespace Kyloe
{
    class Program
    {
        public static void Main()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (input is null)
                    return;

                if (input.StartsWith('$'))
                {
                    if (!EvaluteDollarCommand(input)) return;
                    continue;
                }

                var parser = new Parser(input);

                var tree = parser.Parse();

                foreach (var d in parser.GetDiagnostics())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(d.Message());
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var writer = new PrettyWriter(Console.Out);
                writer.Write(tree);
            }
        }

        private static bool EvaluteDollarCommand(string input)
        {
            if (input.StartsWith("$exit"))
            {
                return false;
            }
            else if (input.StartsWith("$clear"))
            {
                Console.Clear();
                return true;
            }
            else
            {
                Console.WriteLine($"Invalid dollar command: {input}");
                return true;
            }
        }
    }
}