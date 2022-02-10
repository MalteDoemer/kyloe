using System;
using System.Text;
using Kyloe.Diagnostics;
using Kyloe.Syntax;
using Kyloe.Text;

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

                PrintDiagnostics(input, diagnostics);

                var writer = new PrettyWriter(Console.Out);
                writer.Write(tree);
            }
        }

        private static void PrintDiagnostics(string text, DiagnosticCollecter diagnostics)
        {
            var prevColor = Console.ForegroundColor;

            if (diagnostics.HasWarnings())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var warning in diagnostics.GetWarnings())
                    PrintDiagnostic(text, warning);
            }

            if (diagnostics.HasErrors())
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var error in diagnostics.GetErrors())
                    PrintDiagnostic(text, error);
            }

            Console.ForegroundColor = prevColor;
        }

        private static void PrintDiagnostic(string text, Diagnostic diagnostic)
        {
            if (diagnostic.Location is SourceLocation location)
            {
                StringBuilder builder = new StringBuilder();
                Console.WriteLine(text);
                Console.Write(new string(' ', location.Start));
                Console.WriteLine(new String('~', location.Length));
            }

            Console.WriteLine(diagnostic.Message());
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