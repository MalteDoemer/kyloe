using System;
using System.Text;
using Kyloe.Diagnostics;
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

                var diagnostics = new DiagnosticCollecter();

                var parser = new Parser(input, diagnostics);
                var tree = parser.Parse();

                PrintDiagnostics(diagnostics);

                var writer = new PrettyWriter(Console.Out);
                writer.Write(tree);
            }
        }

        private static void PrintDiagnostics(DiagnosticCollecter diagnostics)
        {
            var prevColor = Console.ForegroundColor;

            if (diagnostics.HasWarnings())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var warning in diagnostics.GetWarnings())
                    Console.WriteLine(warning.Message());
            }

            if (diagnostics.HasErrors())
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var warning in diagnostics.GetErrors())
                    Console.WriteLine(warning.Message());
            }

            Console.ForegroundColor = prevColor;
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